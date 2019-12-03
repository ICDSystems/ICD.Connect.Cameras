using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Xml;
using ICD.Connect.Cameras.Devices;
using ICD.Connect.Devices.Windows;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Cameras.Windows
{
	[KrangSettings("WindowsUsbCamera", typeof(WindowsUsbCameraDevice))]
	public sealed class WindowsUsbCameraDeviceSettings : AbstractCameraDeviceSettings
	{
		private const string ELEMENT_INSTANCE_ID = "DevicePath";

		public WindowsDevicePathInfo DevicePath { get; set; }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(ELEMENT_INSTANCE_ID, DevicePath.ToString());
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			string devicePathString = XmlUtils.TryReadChildElementContentAsString(xml, ELEMENT_INSTANCE_ID);
			WindowsDevicePathInfo devicePath = default(WindowsDevicePathInfo);

			try
			{
				devicePath = new WindowsDevicePathInfo(devicePathString);
			}
			catch (Exception e)
			{
				Logger.AddEntry(eSeverity.Error, "Failed to read device path {0} - {1}",
				                StringUtils.ToRepresentation(devicePathString), e.Message);
			}

			DevicePath = devicePath;
		}
	}
}