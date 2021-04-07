using System;
using System.Collections.Generic;
using Crestron.SimplSharpPro.DeviceSupport;

using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash_Essentials_Core.Queues;
using PepperDash.Essentials.Devices.Common.Cameras;
using Crestron.SimplSharp;

using Visca;

namespace PDE.CameraViscaPlugin.EPI
{
    public partial class CameraVisca : CameraBase, IBridgeAdvanced, ICommunicationMonitor, IHasPowerControlWithFeedback,
        IHasCameraOff, IHasCameraPtzControl, IHasCameraFocusControl, IHasAutoFocusMode, IHasCameraMute, IHasCameraPresets
    {
        private readonly CameraViscaConfig _config;
		private readonly IBasicCommunication _comms;
		private readonly GenericCommunicationMonitor _commsMonitor;
        private readonly ViscaProtocolProcessor _visca;
        private readonly ViscaCamera _camera;
        public ViscaCamera Camera { get { return _camera; } }

        /// <summary>
		/// CameraVisca Plugin device constructor using IBasicCommunication
		/// </summary>
		/// <param name="key"></param>
		/// <param name="name"></param>
		/// <param name="config"></param>
		/// <param name="comms"></param>
		public CameraVisca(string key, string name, CameraViscaConfig config, IBasicCommunication comm)
			: base(key, name)
		{
			Debug.Console(0, this, "Constructing new {0} instance", name);

			_config = config;
            Enabled = _config.Enabled;

            _visca = new ViscaProtocolProcessor(comm.SendBytes,  new Action<byte, string, object[]>( (l, f, o) => 
                {
                    Debug.Console(l, this, f, o);
                }));

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
            _camera.MuteChanged += (o, e) => { CameraIsMutedFeedback.FireUpdate(); };

            _comms = comm;
			var socket = _comms as ISocketStatus;
			if (socket != null)
			{
				// device comms is IP **ELSE** device comms is RS232
				socket.ConnectionChange += socket_ConnectionChange;
				Connect = true;
            }

            if (_config.CommunicationMonitorProperties != null)
            {
                _commsMonitor = new GenericCommunicationMonitor(this, _comms,
                    _config.CommunicationMonitorProperties.PollInterval,
                    _config.CommunicationMonitorProperties.TimeToWarning,
                    _config.CommunicationMonitorProperties.TimeToError,
                    _camera.Poll);
            }
            else
            {
                _commsMonitor = new GenericCommunicationMonitor(this, _comms, 10000, 20000, 30000, _camera.Poll);
            }

            _commsMonitor.Client.BytesReceived += (s, e) => { if (Enabled) _visca.ProcessIncomingData(e.Bytes); };
        }

        public override bool CustomActivate()
        {
            _commsMonitor.StatusChange += (o, a) => { Debug.Console(2, this, "Communication monitor state: {0}", _commsMonitor.Status); };
            Connect = true;
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
            var joinMap = new CameraViscaBridgeJoinMap(joinStart);

            // This adds the join map to the collection on the bridge
            if (bridge != null)
            {
                bridge.AddJoinMap(Key, joinMap);
            }

            var customJoins = JoinMapHelper.TryGetJoinMapAdvancedForDevice(joinMapKey);

            if (customJoins != null)
            {
                joinMap.SetCustomJoinData(customJoins);
            }

            Debug.Console(1, "Linking to Trilist '{0}'", trilist.ID.ToString("X"));
            Debug.Console(0, "Linking to Bridge Type {0}", GetType().Name);

            // TODO [ ] Implement bridge links as needed

            // links to bridge
            trilist.SetString(joinMap.DeviceName.JoinNumber, Name);

            trilist.SetBoolSigAction(joinMap.Connect.JoinNumber, sig => Connect = sig);
            ConnectFeedback.LinkInputSig(trilist.BooleanInput[joinMap.Connect.JoinNumber]);

            StatusFeedback.LinkInputSig(trilist.UShortInput[joinMap.Status.JoinNumber]);
            OnlineFeedback.LinkInputSig(trilist.BooleanInput[joinMap.IsOnline.JoinNumber]);

            UpdateFeedbacks();

            trilist.OnlineStatusChange += (o, a) =>
            {
                if (!a.DeviceOnLine) return;

                trilist.SetString(joinMap.DeviceName.JoinNumber, Name);
                UpdateFeedbacks();
            };
        }

        private void UpdateFeedbacks()
        {
            // TODO [ ] Update as needed for the plugin being developed
            ConnectFeedback.FireUpdate();
            OnlineFeedback.FireUpdate();
            StatusFeedback.FireUpdate();
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

        public void PanLeft()  { _camera.Left(); }

        public void PanRight()  { _camera.Right(); }

        public void PanStop()  { _camera.Stop(); }

        #endregion

        #region IHasCameraTiltControl Members

        public void TiltDown() { _camera.Down(); }

        public void TiltStop()  { _camera.Stop(); }

        public void TiltUp() { _camera.Up(); }

        #endregion

        #region IHasCameraZoomControl Members

        public void ZoomIn() { _camera.ZoomTele(); }

        public void ZoomOut() { _camera.ZoomWide(); }

        public void ZoomStop() { _camera.ZoomStop(); }

        #endregion IHasCameraZoomControl Members

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

        public void PresetSelect(int preset) { _camera.MemoryRecall((byte) preset);}

        public void PresetStore(int preset, string description)
        {
            throw new NotImplementedException();
        }

        public List<CameraPreset> Presets { get { return _config.Presets; } }

        public event EventHandler<EventArgs> PresetsListHasChanged;

        #endregion

    }
}

