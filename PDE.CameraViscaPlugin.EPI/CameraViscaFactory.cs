using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

using Newtonsoft.Json;

using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace PDE.CameraViscaPlugin.EPI
{
	/// <summary>
	/// CameraVisca Plugin device factory using IBasicCommunication
	/// </summary>
    public class CameraViscaFactory : EssentialsPluginDeviceFactory<CameraVisca>
    {
		/// <summary>
		/// CameraVisca plugin device factory constructor
		/// </summary>
        public CameraViscaFactory()
        {
            // Set the minimum Essentials Framework Version
			MinimumEssentialsFrameworkVersion = "1.7.5";

            // In the constructor we initialize the list with the typenames that will build an instance of this device
            TypeNames = new List<string>() { "cameravisca2", "roboshot-12", "roboshot-30" };
        }
        
		/// <summary>
		/// Builds and returns an instance of CameraVisca
		/// </summary>
		/// <param name="dc">device configuration</param>
		/// <returns>plugin device or null</returns>
		/// <seealso cref="PepperDash.Core.eControlMethod"/>
        public override EssentialsDevice BuildDevice(DeviceConfig dc)
        {
            Debug.Console(1, "[{0}] Factory Attempting to create new CameraVisca device from type: {1}", dc.Key, dc.Type);

            IList<string> deserializeErrorMessages = new List<string>();

            // get the plugin device properties configuration object & check for null 
            var cameraViscaConfig = JsonConvert.DeserializeObject<CameraViscaConfig>(
                dc.Properties.ToString(), new JsonSerializerSettings()
                {
                    Error = delegate(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
                    {
                        deserializeErrorMessages.Add(args.ErrorContext.Error.Message);
                        args.ErrorContext.Handled = true;
                    }
                });
            if (deserializeErrorMessages.Count > 0)
            {
                Debug.Console(0, "[{0}] Factory: failed to parse config: {1}", dc.Key, String.Join("\r\n", deserializeErrorMessages.ToArray()));
                return null;
            }
            if (cameraViscaConfig == null)
            {
                Debug.Console(0, "[{0}] Factory: failed to read properties config for {1}", dc.Key, dc.Name);
                return null;
            }

            // attempt build the plugin device comms device & check for null
            var comm = CommFactory.CreateCommForDevice(dc);
            if (comm == null)
            {
                Debug.Console(1, "[{0}] Factory Notice: No control object present for device {1}", dc.Key, dc.Name);
                return null;
            }

            return new CameraVisca(dc.Key, dc.Name, cameraViscaConfig, comm);
        }
    }
}

          