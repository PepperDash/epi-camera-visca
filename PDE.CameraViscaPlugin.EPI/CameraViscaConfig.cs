using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Devices.Common.Cameras;

namespace PDE.CameraViscaPlugin.EPI
{
	/// <summary>
	/// CameraVisca Plugin configuration object
	/// </summary>
	[ConfigSnippet("\"properties\":{\"control\":{}")]
	public class CameraViscaConfig
	{
        /// <summary>
        /// Control ID of the camera (1-7)
        /// </summary>
        [JsonProperty("id")]
        public byte Id
        {
            get
            {
                return id;
            }
            set
            {
                if (value > 0 && value < 8)
                    id = value;
                else
                    throw (new ArgumentOutOfRangeException("Id", "Camera ID should be in range between 1 to 7"));
            }
        }
        private byte id;

        /// <summary>
        /// Home VISCA command support. Not all cameras support this command, if not supported, absolute position will be used.
        /// </summary>
        [JsonProperty("homeCmdSupport")]
        public bool HomeCmdSupport { get; set; }

        /// <summary>
        /// Slow Pan speed (0-18)
        /// </summary>
        [JsonProperty("panSpeedSlow")]
        public byte PanSpeedSlow { get; set; }

        /// <summary>
        /// Fast Pan speed (0-18)
        /// </summary>
        [JsonProperty("panSpeedFast")]
        public byte PanSpeedFast { get; set; }

        /// <summary>
        /// Slow tilt speed (0-18)
        /// </summary>
        [JsonProperty("tiltSpeedSlow")]
        public byte TiltSpeedSlow { get; set; }

        /// <summary>
        /// Fast tilt speed (0-18)
        /// </summary>
        [JsonProperty("tiltSpeedFast")]
        public byte TiltSpeedFast { get; set; }

        /// <summary>
        /// Time a button must be held before fast speed is engaged (Milliseconds)
        /// </summary>
        [JsonProperty("fastSpeedHoldTimeMs")]
        public byte FastSpeedHoldTimeMs { get; set; }

        [JsonProperty("supportsAutoMode")]
        public bool SupportsAutoMode { get; set; }

        [JsonProperty("supportsOffMode")]
        public bool SupportsOffMode { get; set; }

        [JsonProperty("presets")]
        public List<CameraPreset> Presets { get; set; }

		/// <summary>
		/// Essentials control config object
		/// </summary>
		[JsonProperty("control")]
		public EssentialsControlPropertiesConfig Control { get; set; }

        /// <summary>
        /// CommunicationMonitor config object
        /// </summary>
        public CommunicationMonitorConfig CommunicationMonitorProperties { get; set; }

		/// <summary>
		/// Constuctor
		/// </summary>
		/// <remarks>
		/// If using a collection you must instantiate the collection in the constructor
		/// to avoid exceptions when reading the configuration file 
		/// </remarks>
		public CameraViscaConfig()
		{
			//DeviceDictionary = new Dictionary<string, EssentialsPluginConfigObjectDictionaryTemplate>();
		}
	}
}