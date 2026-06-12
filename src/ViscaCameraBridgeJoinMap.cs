using PepperDash.Essentials.Core;

namespace ViscaCameraPlugin
{
	public class ViscaCameraBridgeJoinMap : JoinMapBaseAdvanced
	{
		#region Digital

		[JoinName("TiltUp")]
		public JoinDataComplete TiltUp = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 1,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Tilt Up",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		[JoinName("TiltDown")]
		public JoinDataComplete TiltDown = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 2,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Tilt Down",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		[JoinName("PanLeft")]
		public JoinDataComplete PanLeft = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 3,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Pan Left",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		[JoinName("PanRight")]
		public JoinDataComplete PanRight = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 4,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Pan Right",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		[JoinName("ZoomIn")]
		public JoinDataComplete ZoomIn = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 5,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Zoom In",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		[JoinName("ZoomOut")]
		public JoinDataComplete ZoomOut = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 6,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Zoom Out",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});
     
		[JoinName("PowerOn")]
		public JoinDataComplete PowerOn = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 7,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Camera power on",
				JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
				JoinType = eJoinType.Digital
			});

		[JoinName("PowerOff")]
		public JoinDataComplete PowerOff = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 8,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Camera power off",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		[JoinName("IsOnline")]
		public JoinDataComplete IsOnline = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 9,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Is Online",
				JoinCapabilities = eJoinCapabilities.ToSIMPL,
				JoinType = eJoinType.Digital
			});

		[JoinName("Home")]
		public JoinDataComplete Home = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 10,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Home",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		[JoinName("PresetSelect")]
		public JoinDataComplete PresetSelect = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 11,
				JoinSpan = 16
			},
			new JoinMetadata
			{
				Description = "Preset select (press), store (hold)",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

        [JoinName("FocusNear")]
        public JoinDataComplete FocusNear = new JoinDataComplete(
            new JoinData
            {
                JoinNumber = 28,
                JoinSpan = 1
            },
            new JoinMetadata
            {
                Description = "Focus Near",
                JoinCapabilities = eJoinCapabilities.FromSIMPL,
                JoinType = eJoinType.Digital
            });

        [JoinName("FocusFar")]
        public JoinDataComplete FocusFar = new JoinDataComplete(
            new JoinData
            {
                JoinNumber = 29,
                JoinSpan = 1
            },
            new JoinMetadata
            {
                Description = "FocusFar",
                JoinCapabilities = eJoinCapabilities.FromSIMPL,
                JoinType = eJoinType.Digital
            });

		/// <summary>
		/// Camera preset saved
		/// </summary>
		[JoinName("PresetStoredFeedback")]
		public JoinDataComplete PresetStoredFeedback = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 30,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Camera preset stored Feedback",
				JoinCapabilities = eJoinCapabilities.ToSIMPL,
				JoinType = eJoinType.Digital
			});


		[JoinName("PresetStore")]
		public JoinDataComplete PresetStore = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 31,
				JoinSpan = 16
			},
			new JoinMetadata
			{
				Description = "Preset store",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});


        [JoinName("TrackingOn")]
        public JoinDataComplete TrackingOn = new JoinDataComplete(
    new JoinData
    {
        JoinNumber = 46,
        JoinSpan = 1
    },
    new JoinMetadata
    {
        Description = "Tracking On",
        JoinCapabilities = eJoinCapabilities.FromSIMPL,
        JoinType = eJoinType.Digital
    });

        [JoinName("TrackingOff")]
        public JoinDataComplete TrackingOff = new JoinDataComplete(
            new JoinData
            {
                JoinNumber = 47,
                JoinSpan = 1
            },
            new JoinMetadata
            {
                Description = "Tracking Off",
                JoinCapabilities = eJoinCapabilities.FromSIMPL,
                JoinType = eJoinType.Digital
            });


        [JoinName("PrivacyOn")]
		public JoinDataComplete PrivacyOn = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 48,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Privacy On",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		[JoinName("PrivacyOff")]
		public JoinDataComplete PrivacyOff = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 49,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Privacy Off",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Digital
			});

		[JoinName("TriggerAutoFocus")]
		public JoinDataComplete TriggerAutoFocus = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 50,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Trigger auto focus",
				JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
				JoinType = eJoinType.Digital
			});

		#endregion


		#region Analog

		[JoinName("PanSpeed")]
		public JoinDataComplete PanSpeed = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 1,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Pan Speed",
				JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
				JoinType = eJoinType.Analog
			});

		[JoinName("TiltSpeed")]
		public JoinDataComplete TiltSpeed = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 2,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Tilt Speed",
				JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
				JoinType = eJoinType.Analog
			});

		[JoinName("ZoomSpeed")]
		public JoinDataComplete ZoomSpeed = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 3,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Zoom Speed",
				JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
				JoinType = eJoinType.Analog
			});

        [JoinName("FocusSpeed")]
        public JoinDataComplete FocusSpeed = new JoinDataComplete(
            new JoinData
            {
                JoinNumber = 4,
                JoinSpan = 1
            },
            new JoinMetadata
            {
                Description = "Focus Speed",
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.Analog
            });

		[JoinName("NumberOfPresets")]
		public JoinDataComplete NumberOfPresets = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 11,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Number of configured presets",
				JoinCapabilities = eJoinCapabilities.ToSIMPL,
				JoinType = eJoinType.Analog
			});

		[JoinName("PresetSelectByNumber")]
		public JoinDataComplete PresetSelectByNumber = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 11,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Preset select by number",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Analog
			});

		[JoinName("PresetStoreByNumber")]
		public JoinDataComplete PresetStoreByNumber = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 12,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Preset store by number",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Analog
			});

		[JoinName("PresetRawSelect")]
		public JoinDataComplete PresetRawSelect = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 13,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Preset recall by raw VISCA preset number",
				JoinCapabilities = eJoinCapabilities.FromSIMPL,
				JoinType = eJoinType.Analog
			});

		[JoinName("SocketStatus")]
		public JoinDataComplete SocketStatus = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 50,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Returns Socket Status when using VISCA-over-IP",
				JoinCapabilities = eJoinCapabilities.ToSIMPL,
				JoinType = eJoinType.Analog
			});

		#endregion


		#region Serial

		[JoinName("DeviceName")]
		public JoinDataComplete DeviceName = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 1,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Name",
				JoinCapabilities = eJoinCapabilities.ToSIMPL,
				JoinType = eJoinType.Serial
			});

		//[JoinName("IPAddress")]
		//public JoinDataComplete IpAddress = new JoinDataComplete(
		//    new JoinData
		//    {
		//        JoinNumber = 2,
		//        JoinSpan = 1
		//    },
		//    new JoinMetadata
		//    {
		//        Description = "Camera IP address",
		//        JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
		//        JoinType = eJoinType.Serial
		//    });

		[JoinName("PresetName")]
		public JoinDataComplete PresetNames = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 11,
				JoinSpan = 16
			},
			new JoinMetadata
			{
				Description = "Preset Name",
				JoinCapabilities = eJoinCapabilities.ToSIMPL,
				JoinType = eJoinType.Serial
			});

		[JoinName("DeviceComs")]
		public JoinDataComplete DeviceComs = new JoinDataComplete(
			new JoinData
			{
				JoinNumber = 50,
				JoinSpan = 1
			},
			new JoinMetadata
			{
				Description = "Camera device communications",
				JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
				JoinType = eJoinType.Serial
			});


		#endregion

		public ViscaCameraBridgeJoinMap(uint joinStart)
			: base(joinStart, typeof(ViscaCameraBridgeJoinMap))
		{
		}
	}
}