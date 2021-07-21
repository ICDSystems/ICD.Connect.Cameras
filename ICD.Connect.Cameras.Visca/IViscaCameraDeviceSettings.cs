using ICD.Connect.Cameras.Devices;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Protocol.Settings;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Cameras.Visca
{
	public interface IViscaCameraDeviceSettings : ICameraDeviceSettings, IComSpecSettings
	{
		[OriginatorIdSettingsProperty(typeof(ISerialPort))]
		int? Port { get; set; }

		int? PanTiltSpeed { get; set; }

		int? ZoomSpeed { get; set; }
	}
}
