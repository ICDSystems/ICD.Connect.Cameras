using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Cameras.Visca
{
	[KrangSettings("ViscaCamera", typeof(ViscaCameraDevice))]
	public sealed class ViscaCameraDeviceSettings : AbstractViscaCameraDeviceSettings
	{
	}
}
