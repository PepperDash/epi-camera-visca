using System.Collections.Generic;
using Newtonsoft.Json;
using PepperDash.Essentials.Core;

namespace ViscaCameraPlugin
{
	public class ViscaCameraConfig
	{
		[JsonProperty("control")]
		public EssentialsControlPropertiesConfig Control { get; set; }

		[JsonProperty("address")]
		public uint Address { get; set; }

		[JsonProperty("panSpeed")]
		public uint PanSpeed { get; set; }

		[JsonProperty("tiltSpeed")]
		public uint TiltSpeed { get; set; }

		[JsonProperty("ZoomSpeed")]
		public uint ZoomSpeed { get; set; }

		[JsonProperty("FocusSpeed")]
		public uint FocusSpeed { get; set; }

		[JsonProperty("PrivacyOnPreset")]
		public uint PrivacyOnPreset { get; set; }

		[JsonProperty("PrivacyOffPreset")]
		public uint PrivacyOffPreset { get; set; }

		[JsonProperty("pollTimeMs")]
		public int PollTimeMs { get; set; }

		[JsonProperty("presets")]
		public List<ViscaCameraPresetsConfig> Presets { get; set; }

		[JsonProperty("capabilities")]
		public CameraCapabilities Capabilities { get; set; }

		public ViscaCameraConfig()
		{
			Presets = new List<ViscaCameraPresetsConfig>();
		}
	}

	public class ViscaCameraPresetsConfig
	{
		[JsonProperty("name")]
		public string Name { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }
	}

	public class CameraCapabilities
	{
        /// <summary>
        /// Indicates whether the camera can pan
        /// </summary>
        [JsonProperty("canPan", NullValueHandling = NullValueHandling.Ignore)]
        public bool CanPan { get; set; }

        /// <summary>
        /// Indicates whether the camera can tilt
        /// </summary>
        [JsonProperty("canTilt", NullValueHandling = NullValueHandling.Ignore)]
        public bool CanTilt { get; set; }

        /// <summary>
        /// Indicates whether the camera can zoom
        /// </summary>
        [JsonProperty("canZoom", NullValueHandling = NullValueHandling.Ignore)]
        public bool CanZoom { get; set; }

        /// <summary>
        /// Indicates whether the camera can focus
        /// </summary>
        [JsonProperty("canFocus", NullValueHandling = NullValueHandling.Ignore)]
        public bool CanFocus { get; set; }
	}
}