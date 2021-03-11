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
    public class CameraViscaConfig : CameraPropertiesConfig
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
        /// If Home VISCA command is not supported, use home Pan/Tilt/Zoom position be used.
        /// </summary>
        [JsonProperty("homePanPosition")]
        public int HomePanPosition { get; set; }

        /// <summary>
        /// If Home VISCA command is not supported, use home Pan/Tilt/Zoom position be used.
        /// </summary>
        [JsonProperty("homeTiltPosition")]
        public int HomeTiltPosition { get; set; }

        /// <summary>
        /// If Home VISCA command is not supported, use home Pan/Tilt/Zoom position be used.
        /// </summary>
        [JsonProperty("homeZoomPosition")]
        public int HomeZoomPosition { get; set; }

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