using ICD.Connect.Settings;

namespace ICD.Connect.Devices
{
	public class DeviceAction : OriginatorAction
	{
		protected DeviceAction(string val) : base(val)
		{
		}

		public static DeviceAction ViewControls { get { return new DeviceAction("DeviceAction.ViewControls"); } }
	}
}