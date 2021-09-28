using System;
using System.Collections.Generic;
using Crestron.SimplSharpPro.DeviceSupport;

using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Devices.Common.Cameras;
using Crestron.SimplSharp;

using Visca;

namespace PDE.CameraViscaPlugin.EPI
{
    public class CameraVisca : CameraBase, IBridgeAdvanced, ICommunicationMonitor, IHasPowerControlWithFeedback,
        IHasCameraOff, IHasCameraPtzControl, IHasCameraFocusControl, IHasAutoFocusMode, IHasCameraMute, IHasCameraPresets
    {
        private readonly CameraViscaConfig _config;
		private readonly IBasicCommunication _comms;
		private readonly GenericCommunicationMonitor _commsMonitor;
        private readonly ViscaProtocolProcessor _visca;
        private readonly ViscaCamera _camera;
        /// <summary>
        /// Holds all defined Camera Poll commands and it's string key
        /// </summary>
        private readonly Dictionary<string, Action> _cameraPollCommands;
        /// <summary>
        /// Holds configured to poll commands
        /// </summary>
        private readonly List<Action> _poll = new List<Action>();

        /// <summary>
        /// Used to determine when to move the camera at a faster speed if a direction is held
        /// </summary>
        private readonly bool _ptzSpeedIncreaseBehaivor;
        private readonly CTimerCallbackFunction _ptzSpeedIncreaseAction;
        private CTimer _ptzSpeedIncreaseTimer;
        private readonly byte _ptzPanNormalSpeed;
        private readonly byte _ptzTiltNormalSpeed;

        public ViscaCamera Camera { get { return _camera; } }

        /// <summary>
		/// CameraVisca Plugin device constructor using IBasicCommunication
		/// </summary>
		/// <param name="key"></param>
		/// <param name="name"></param>
		/// <param name="config"></param>
		/// <param name="comm"></param>
		public CameraVisca(string key, string name, CameraViscaConfig config, IBasicCommunication comm)
			: base(key, name)
		{
			Debug.Console(0, this, "Constructing new {0} instance", name);

			_config = config;
            Enabled = _config.Enabled;

            _visca = new ViscaProtocolProcessor(comm.SendBytes,  (l, f, o) => Debug.Console(l, this, f, o));

            _camera = new ViscaCamera((ViscaCameraId)_config.Id, null, _visca);
            _camera.PollEnabled = Enabled;

            Capabilities = eCameraCapabilities.Pan | eCameraCapabilities.Tilt | eCameraCapabilities.Zoom | eCameraCapabilities.Focus;
            ControlMode = eCameraControlMode.Auto;

            ConnectFeedback = new BoolFeedback( () => Connect);
			OnlineFeedback = new BoolFeedback( () => _commsMonitor.IsOnline);
			StatusFeedback = new IntFeedback( () => (int)_commsMonitor.Status);
            PowerIsOnFeedback = new BoolFeedback( () => _camera.Power);
            CameraIsOffFeedback = new BoolFeedback( () => !_camera.Power);
            CameraIsMutedFeedback = new BoolFeedback( () => _camera.Mute);

            _camera.PowerChanged += (o, e) => { PowerIsOnFeedback.FireUpdate(); CameraIsOffFeedback.FireUpdate(); };
            _camera.MuteChanged += (o, e) => CameraIsMutedFeedback.FireUpdate();

            _cameraPollCommands = new Dictionary<string, Action>()
            {
                {"AE",            _camera.AEPoll},
                {"Aperture",      _camera.AperturePoll},
                {"BackLight",     _camera.BackLightPoll},
                {"BGain",         _camera.BGainPoll},
                {"ExpComp",       _camera.ExpCompPoll},
                {"FocusAuto",     _camera.FocusAutoPoll},
                {"FocusPosition", _camera.FocusPositionPoll},
                {"Gain",          _camera.GainPoll},
                {"Iris",          _camera.IrisPoll},
                {"Mute",          _camera.MutePoll},
                {"PTZPosition",   _camera.PanTiltPositionPoll},
                {"Power",         _camera.PowerPoll},
                {"RGain",         _camera.RGainPoll},
                {"Shutter",       _camera.ShutterPoll},
                {"Title",         _camera.TitlePoll},
                {"WB",            _camera.WBModePoll},
                {"WD",            _camera.WideDynamicModePoll},
                {"ZoomPosition",  _camera.ZoomPositionPoll},
            };

            _comms = comm;

            // TODO: For VISCA camera current library implementation only
            // serial is supported, so socket code is not needed
			var socket = _comms as ISocketStatus;
			if (socket != null)
			{
				socket.ConnectionChange += socket_ConnectionChange;
            }

            if (_config.CommunicationMonitorProperties != null)
            {
                if (!String.IsNullOrEmpty(_config.CommunicationMonitorProperties.PollString))
                    foreach (var poll in _config.CommunicationMonitorProperties.PollString.Split(','))
                        if (_cameraPollCommands.ContainsKey(poll.Trim()))
                            _poll.Add(_cameraPollCommands[poll.Trim()]);

                _commsMonitor = new GenericCommunicationMonitor(this, _comms,
                    _config.CommunicationMonitorProperties.PollInterval,
                    _config.CommunicationMonitorProperties.TimeToWarning,
                    _config.CommunicationMonitorProperties.TimeToError,
                    () => _poll.ForEach((pollAction) => pollAction()) 
                    );
            }
            else
            {
                // We do not have CommunicationMonitorProperties and therefore
                // no Poll string defined, using ALL poll functions
                _poll.AddRange(_cameraPollCommands.Values);
                _commsMonitor = new GenericCommunicationMonitor(this, _comms, 10000, 20000, 30000, () => _poll.ForEach((pollAction) => pollAction()));
            }

            _commsMonitor.Client.BytesReceived += (s, e) => _visca.ProcessIncomingData(e.Bytes);
            DeviceManager.AddDevice(CommunicationMonitor);

            DeviceManager.AllDevicesActivated += (sender, args) =>
            {
                if (Enabled)
                {
                    Connect = true;
                }
            };

            // Handle Increase PTZ move speed
            #region PTZ Speed Increase

            // if FastSpeedHoldTimeMs defined in config, enable SpeedIncrease behaivor
            if (_config.FastSpeedHoldTimeMs > 0)
            {
                _ptzSpeedIncreaseBehaivor = true;

                if (_config.PanSpeedSlow > 0)
                    _camera.PanSpeed = _config.PanSpeedSlow;

                if (_config.TiltSpeedSlow > 0)
                    _camera.TiltSpeed = _config.TiltSpeedSlow;

                _ptzPanNormalSpeed = _camera.PanSpeed;
                _ptzTiltNormalSpeed = _camera.TiltSpeed;

                _ptzSpeedIncreaseAction = ptzCommand =>
                {
                    // Kill Speed Increase Timer as we already in fast pace
                    ptzSpeedChangeTimerDispose();
                    // if Fast speeds not defined, use Max values
                    _camera.PanSpeed = (_config.PanSpeedFast > 0) ? _config.PanSpeedFast : Visca.ViscaDefaults.PanSpeedLimits.High;
                    _camera.TiltSpeed = (_config.TiltSpeedFast > 0) ? _config.PanSpeedFast : Visca.ViscaDefaults.TiltSpeedLimits.High;
                    // we passed current ptz command to action, so we repeat it with increased speed 
                    (ptzCommand as Action).Invoke();
                };
            }

            #endregion PTZ Speed Increase
        }

        public override bool CustomActivate()
        {
            _commsMonitor.StatusChange += (o, a) => Debug.Console(2, this, "Communication monitor state: {0}", _commsMonitor.Status);
            
            return true;
        }

        #region Communications handlers

        private void socket_ConnectionChange(object sender, GenericSocketStatusChageEventArgs args)
		{
			if (ConnectFeedback != null)
				ConnectFeedback.FireUpdate();

			if (StatusFeedback != null)
				StatusFeedback.FireUpdate();
        }

        #endregion Communications handlers

        #region Feedbacks

        /// <summary>
        /// Connects/disconnects the comms of the plugin device
        /// </summary>
        /// <remarks>
        /// triggers the _comms.Connect/Disconnect as well as thee comms monitor start/stop
        /// </remarks>
        public bool Connect
        {
            get { return _comms.IsConnected; }
            set
            {
                if (value)
                {
                    _comms.Connect();
                    _commsMonitor.Start();
                }
                else
                {
                    _comms.Disconnect();
                    _commsMonitor.Stop();
                }
            }
        }

        /// <summary>
        /// Reports connect feedback through the bridge
        /// </summary>
        public BoolFeedback ConnectFeedback { get; private set; }

        /// <summary>
        /// Reports online feedback through the bridge
        /// </summary>
        public BoolFeedback OnlineFeedback { get; private set; }

        /// <summary>
        /// Reports socket status feedback through the bridge
        /// </summary>
        public IntFeedback StatusFeedback { get; private set; }

        #endregion Feedbacks

        #region Overrides of EssentialsBridgeableDevice

        /// <summary>
        /// Links the plugin device to the EISC bridge
        /// </summary>
        /// <param name="trilist"></param>
        /// <param name="joinStart"></param>
        /// <param name="joinMapKey"></param>
        /// <param name="bridge"></param>
        public void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
        {
            LinkCameraToApi(this, trilist, joinStart, joinMapKey, bridge);
        }

        #endregion

        #region ICommunicationMonitor Members

        public StatusMonitorBase CommunicationMonitor { get { return _commsMonitor; } }

        #endregion

        #region IHasPowerControlWithFeedback Members

        public BoolFeedback PowerIsOnFeedback { get; private set; }

        #region IHasPowerControl Members

        public void PowerOff() { _camera.Power = false; }

        public void PowerOn() { _camera.Power = true; }

        public void PowerToggle() { _camera.Power = !_camera.Power; }

        #endregion IHasPowerControl Members

        #endregion IHasPowerControlWithFeedback Members

        #region IHasCameraOff Members

        public BoolFeedback CameraIsOffFeedback { get; private set; }

        public void CameraOff() { _camera.Power = false; }

        #endregion

        #region IHasCameraPtzControl Members

        public void PositionHome()
        {
            if (_config.HomeCmdSupport)
                _camera.Home();
            else
            {
                _camera.PositionAbsolute(_config.HomePanPosition, _config.HomeTiltPosition);
                _camera.ZoomPosition = _config.HomeZoomPosition;
            }
        }

        #region IHasCameraPanControl Members

        public void PanLeft() { ptzCommand((Action)_camera.Left); }

        public void PanRight() { ptzCommand((Action)_camera.Right); }

        public void PanStop() { ptzStop(); }

        #endregion

        #region IHasCameraTiltControl Members

        public void TiltDown() { ptzCommand((Action)_camera.Down); }

        public void TiltUp() { ptzCommand((Action)_camera.Up); }

        public void TiltStop() { ptzStop(); }

        #endregion

        #region IHasCameraZoomControl Members

        public void ZoomIn() { _camera.ZoomTele(); }

        public void ZoomOut() { _camera.ZoomWide(); }

        public void ZoomStop() { _camera.ZoomStop(); }

        #endregion IHasCameraZoomControl Members

        #region PTZ SpeedIncrease Behaivor Implementation

        private void ptzCommand(Action ptzCommand)
        {
            if (_ptzSpeedIncreaseBehaivor)
            {
                ptzSpeedChangeTimerDispose();
                ptzSpeedReset();
                _ptzSpeedIncreaseTimer = new CTimer(_ptzSpeedIncreaseAction, ptzCommand, _config.FastSpeedHoldTimeMs);
            }
            ptzCommand.Invoke();
        }

        private void ptzStop()
        {
            if (_ptzSpeedIncreaseBehaivor)
            {
                ptzSpeedChangeTimerDispose();
                ptzSpeedReset();
            }
            _camera.Stop();
        }

        private void ptzSpeedChangeTimerDispose()
        {
            if (_ptzSpeedIncreaseTimer != null)
            {
                _ptzSpeedIncreaseTimer.Stop();
                _ptzSpeedIncreaseTimer.Dispose();
                _ptzSpeedIncreaseTimer = null;
            }
        }

        private void ptzSpeedReset()
        {
            _camera.PanSpeed = _ptzPanNormalSpeed;
            _camera.TiltSpeed = _ptzTiltNormalSpeed;
        }

        #endregion PTZ SpeedIncrease Behaivor Implementation

        #endregion IHasCameraPtzControl Members

        #region IHasCameraFocusControl Members

        public void FocusFar() { _camera.FocusFar(); }

        public void FocusNear() { _camera.FocusNear(); }

        public void FocusStop() { _camera.FocusStop(); }

        public void TriggerAutoFocus() { _camera.FocusTrigger(); }

        #endregion

        #region IHasAutoFocusMode Members

        public void SetFocusModeAuto() { _camera.FocusAuto = true; }

        public void SetFocusModeManual() { _camera.FocusAuto = false; }

        public void ToggleFocusMode() { _camera.FocusAutoToggle(); }

        #endregion

        #region IHasCameraMute Members

        public BoolFeedback CameraIsMutedFeedback { get; private set; }

        public void CameraMuteOff() { _camera.Mute = false; }

        public void CameraMuteOn() { _camera.Mute = true; }

        public void CameraMuteToggle() { _camera.Mute = !_camera.Mute; }

        #endregion

        #region IHasCameraPresets Members

        public void PresetSelect(int preset) { _camera.MemoryRecall((byte) preset); }

        public void PresetStore(int preset, string description) { _camera.MemorySet((byte) preset); }

        public List<CameraPreset> Presets { get { return _config.Presets; } }

        public event EventHandler<EventArgs> PresetsListHasChanged;

        #endregion

    }
}

