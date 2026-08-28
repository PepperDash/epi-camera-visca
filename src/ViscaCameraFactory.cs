using System.Collections.Generic;
using PepperDash.Core;
using PepperDash.Essentials.Core;


namespace ViscaCameraPlugin
{
    public class ViscaCameraFactory : EssentialsPluginDeviceFactory<ViscaCameraDevice>
    {
        public ViscaCameraFactory()
        {
            // Set the minimum Essentials Framework Version
            MinimumEssentialsFrameworkVersion = "3.0.0";

            // In the constructor we initialize the list with the typenames that will build an instance of this device
            TypeNames = new List<string> { "visca", "viscacamera" };
        }

        // Builds and returns an instance of EssentialsPluginDeviceTemplate
        public override EssentialsDevice BuildDevice(PepperDash.Essentials.Core.Config.DeviceConfig dc)
        {
            Debug.LogDebug("Factory Attempting to create new device from type: {0}", dc.Type);			

	        var comms = CommFactory.CreateCommForDevice(dc);
	        if (comms == null)
	        {
		        Debug.LogError("[{0}] VISCA Camera: failed to create comms for {1}", dc.Key, dc.Name);
		        return null;
	        }
            
            var propertiesConfig = dc.Properties.ToObject<ViscaCameraConfig>();
	        if (propertiesConfig == null)
	        {
		        Debug.LogError("[{0}] VISCA Camera: failed to read properties config for {1}", dc.Key, dc.Name);
		        return null;
	        }

	        try
	        {
		        return new ViscaCameraDevice(dc.Key, dc.Name, comms, propertiesConfig);
	        }
	        catch (System.Exception ex)
	        {
		        Debug.LogError(ex, "[{0}] VISCA Camera: exception constructing device {1}: {2}", dc.Key, dc.Name, ex.Message);
		        return null;
	        }
        }

    }
}