using System;
using System.Collections.Generic;
using Crestron.SimplSharpPro.DeviceSupport;

using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash_Essentials_Core.Queues;
using PepperDash.Essentials.Devices.Common.Cameras;

using Visca;

namespace PDE.CameraViscaPlugin.EPI
{
    public partial class CameraVisca : CameraBase, IBridgeAdvanced, ICommunicationMonitor, IHasPowerControlWithFeedback,
        IHasCameraOff, IHasCameraPtzControl, IHasCameraFocusControl, IHasAutoFocusMode, IHasCameraPresets
    //  HasCameraMute
    {
        private readonly CameraViscaConfig _config;
		private readonly IBasicCommunication _comms;
		private readonly GenericCommunicationMonitor _commsMonitor;
        private readonly ViscaProtocolProcessor _visca;
        private readonly List<ViscaCommand> _pollCommands;

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

            //Enabled = _config.Control.

            _visca = new ViscaProtocolProcessor(comm.SendBytes,  new Action<byte, string, object[]>( (l, f, o) => 
                {
                    Debug.Console(l, this, f, o);
                }));
            
            _powerOnCmd = new ViscaPower(_config.Id, true);
            _powerOffCmd = new ViscaPower(_config.Id, false);
            _powerInquiry = new ViscaPowerInquiry(_config.Id, new Action<bool>(power => { _power = power; PowerIsOnFeedback.FireUpdate(); CameraIsOffFeedback.FireUpdate(); OnPowerChanged(new OnOffEventArgs(power)); }));
            _powerOnOffCmdReply = new Action<ViscaRxPacket>(rxPacket => { if (rxPacket.IsCompletionCommand) _visca.EnqueueCommand(_powerInquiry); });

            _zoomStopCmd = new ViscaZoomStop(_config.Id);
            _zoomTeleCmd = new ViscaZoomTele(_config.Id);
            _zoomWideCmd = new ViscaZoomWide(_config.Id);
            // TODO: Do something about limits... config
            _zoomSpeed = new ViscaZoomSpeed(ViscaDefaults.ZoomSpeedLimits);
            _zoomTeleWithSpeedCmd = new ViscaZoomTeleWithSpeed(_config.Id, _zoomSpeed);
            _zoomWideWithSpeedCmd = new ViscaZoomWideWithSpeed(_config.Id, _zoomSpeed);
            _zoomPositionCmd = new ViscaZoomPosition(_config.Id, 0);
            _zoomPositionInquiry = new ViscaZoomPositionInquiry(_config.Id, new Action<int>(position => { _zoomPosition = position; OnZoomPositionChanged(new PositionEventArgs(position)); }));

            _focusStopCmd = new ViscaFocusStop(_config.Id);
            _focusFarCmd = new ViscaFocusFar(_config.Id);
            _focusNearCmd = new ViscaFocusNear(_config.Id);
            _focusSpeed = new ViscaFocusSpeed(ViscaDefaults.FocusSpeedLimits);
            _focusFarWithSpeedCmd = new ViscaFocusFarWithSpeed(_config.Id, _focusSpeed);
            _focusNearWithSpeedCmd = new ViscaFocusNearWithSpeed(_config.Id, _focusSpeed);
            _focusTriggerCmd = new ViscaFocusTrigger(_config.Id);
            _focusInfinityCmd = new ViscaFocusInfinity(_config.Id);

            _focusNearLimitCmd = new ViscaFocusNearLimit(_config.Id, 0x1000);

            _focusAutoOnCmd = new ViscaFocusAutoOn(_config.Id);
            _focusAutoOffCmd = new ViscaFocusAutoOff(_config.Id);
            _focusAutoToggleCmd = new ViscaFocusAutoToggle(_config.Id);
            _focusAutoInquiry = new ViscaFocusAutoInquiry(_config.Id, new Action<bool>(focusAuto => { _focusAuto = focusAuto; OnFocusAutoChanged(new OnOffEventArgs(focusAuto)); }));
            _focusAutoOnOffCmdReply = new Action<ViscaRxPacket>(rxPacket => { if (rxPacket.IsCompletionCommand) _visca.EnqueueCommand(_focusAutoInquiry); });

            _focusPositionCmd = new ViscaFocusPosition(_config.Id, 0);
            _focusPositionInquiry = new ViscaFocusPositionInquiry(_config.Id, new Action<int>(position => { _focusPosition = position; OnFocusPositionChanged(new PositionEventArgs(position)); }));

            // PTZ Commands
            _ptzHome = new ViscaPTZHome(_config.Id);
            _ptzPanSpeed = new ViscaPanSpeed(ViscaDefaults.PanSpeedLimits);
            _ptzTiltSpeed = new ViscaTiltSpeed(ViscaDefaults.TiltSpeedLimits);
            _ptzStop = new ViscaPTZStop(_config.Id, _ptzPanSpeed, _ptzTiltSpeed);
            _ptzUp = new ViscaPTZUp(_config.Id, _ptzPanSpeed, _ptzTiltSpeed);
            _ptzDown = new ViscaPTZDown(_config.Id, _ptzPanSpeed, _ptzTiltSpeed);
            _ptzLeft = new ViscaPTZLeft(_config.Id, _ptzPanSpeed, _ptzTiltSpeed);
            _ptzRight = new ViscaPTZRight(_config.Id, _ptzPanSpeed, _ptzTiltSpeed);
            _ptzUpLeft = new ViscaPTZUpLeft(_config.Id, _ptzPanSpeed, _ptzTiltSpeed);
            _ptzUpRight = new ViscaPTZUpRight(_config.Id, _ptzPanSpeed, _ptzTiltSpeed);
            _ptzDownLeft = new ViscaPTZDownLeft(_config.Id, _ptzPanSpeed, _ptzTiltSpeed);
            _ptzDownRight = new ViscaPTZDownRight(_config.Id, _ptzPanSpeed, _ptzTiltSpeed);
            _ptzAbsolute = new ViscaPTZPosition(_config.Id, false, _ptzPanSpeed, _ptzTiltSpeed, 0, 0);
            _ptzRelative = new ViscaPTZPosition(_config.Id, true, _ptzPanSpeed, _ptzTiltSpeed, 0, 0);

            // Memory commands
            _memorySetCmd = new ViscaMemorySet(_config.Id, 0);
            _memoryRecallCmd = new ViscaMemoryRecall(_config.Id, 0);

            Capabilities = eCameraCapabilities.Pan | eCameraCapabilities.Tilt | eCameraCapabilities.Zoom | eCameraCapabilities.Focus;
            ControlMode = eCameraControlMode.Auto;

            ConnectFeedback = new BoolFeedback(() => Connect);
			OnlineFeedback = new BoolFeedback(() => _commsMonitor.IsOnline);
			StatusFeedback = new IntFeedback(() => (int)_commsMonitor.Status);
            PowerIsOnFeedback = new BoolFeedback(() => Power);
            CameraIsOffFeedback = new BoolFeedback( () => !Power);

			_comms = comm;
			var socket = _comms as ISocketStatus;
			if (socket != null)
			{
				// device comms is IP **ELSE** device comms is RS232
				socket.ConnectionChange += socket_ConnectionChange;
				Connect = true;
            }

            var pollAction = new Action(() =>
                {
                    if (Enabled)
                    {
                        foreach (var command in _pollCommands)
                            _visca.EnqueueCommand(command);
                    }
                });
            if (_config.CommunicationMonitorProperties != null)
            {
                _commsMonitor = new GenericCommunicationMonitor(this, _comms,
                    _config.CommunicationMonitorProperties.PollInterval,
                    _config.CommunicationMonitorProperties.TimeToWarning,
                    _config.CommunicationMonitorProperties.TimeToError,
                    pollAction);
            }
            else
            {
                _commsMonitor = new GenericCommunicationMonitor(this, _comms, 20000, 120000, 300000, pollAction);
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

        public void PowerOff() { Power = false; }

        public void PowerOn() { Power = true; }

        public void PowerToggle() { Power = !Power; }

        #endregion IHasPowerControl Members

        #endregion IHasPowerControlWithFeedback Members

        #region IHasCameraOff Members

        public BoolFeedback CameraIsOffFeedback { get; private set; }

        public void CameraOff() { Power = false; }

        #endregion

        #region IHasCameraPtzControl Members

        public void PositionHome()
        {
            if (_config.HomeCmdSupport)
                Home();
            else
            {
                PositionAbsolute(_config.HomePanPosition, _config.HomeTiltPosition);
                ZoomPosition = _config.HomeZoomPosition;
            }
        }

        #region IHasCameraPanControl Members

        public void PanLeft()  { Left(); }

        public void PanRight()  { Right(); }

        public void PanStop()  { Stop(); }

        #endregion

        #region IHasCameraTiltControl Members

        public void TiltDown() { Down(); }

        public void TiltStop()  { Stop(); }

        public void TiltUp() { Up(); }

        #endregion

        #region IHasCameraZoomControl Members

        public void ZoomIn() { ZoomTele(); }

        public void ZoomOut() { ZoomWide(); }

        #endregion IHasCameraZoomControl Members

        #endregion IHasCameraPtzControl Members

        #region IHasCameraFocusControl Members

        public void TriggerAutoFocus() { FocusTrigger();}

        #endregion

        #region IHasAutoFocusMode Members

        public void SetFocusModeAuto() { FocusAuto = true; }

        public void SetFocusModeManual() { FocusAuto = false; }

        public void ToggleFocusMode() { FocusAutoToggle(); }

        #endregion


        #region IHasCameraPresets Members

        public void PresetSelect(int preset) { MemoryRecall((byte) preset);}

        public void PresetStore(int preset, string description)
        {
            throw new NotImplementedException();
        }

        public List<CameraPreset> Presets { get { return _config.Presets; } }

        public event EventHandler<EventArgs> PresetsListHasChanged;

        #endregion
    }
}

