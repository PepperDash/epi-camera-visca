using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;

namespace PDE.CameraViscaPlugin.EPI 
{
	/// <summary>
	/// Visca Camera Join Map 
	/// </summary>
	/// <see cref="PepperDash.Essentials.Core.Bridges"/>
    public class CameraViscaBridgeJoinMap : CameraControllerJoinMap
	{
		/// <summary>
		/// Camera Visca Plugin device BridgeJoinMap constructor
		/// </summary>
		/// <param name="joinStart">This will be the join it starts on the EISC bridge</param>
		public CameraViscaBridgeJoinMap(uint joinStart)
			: base(joinStart, typeof(CameraViscaBridgeJoinMap))
		{
		}
	}
}