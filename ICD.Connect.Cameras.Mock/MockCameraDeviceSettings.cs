using System;
using ICD.Common.Properties;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Cameras.Mock
{
	public sealed class MockCameraDeviceSettings : AbstractCameraDeviceSettings
	{
		private const string FACTORY_NAME = "MockCamera";

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(MockCameraDevice); } }

		[PublicAPI, XmlFactoryMethod(FACTORY_NAME)]
		public static MockCameraDeviceSettings FromXml(string xml)
		{
			MockCameraDeviceSettings output = new MockCameraDeviceSettings();
			ParseXml(output, xml);
			return output;
		}
	}
}
