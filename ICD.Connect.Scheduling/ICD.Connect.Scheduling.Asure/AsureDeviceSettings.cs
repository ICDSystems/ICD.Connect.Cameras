using System;
using ICD.Common.Attributes.Properties;
using ICD.Common.Properties;
using ICD.Common.Utils.Xml;
using ICD.Connect.Settings;
using ICD.Connect.Settings.Attributes.Factories;

namespace ICD.Connect.Scheduling.Asure
{
	public sealed class AsureDeviceSettings : AbstractDeviceSettings
	{
		private const string FACTORY_NAME = "AsureService";

		private const string RESOURCE_ID_ELEMENT = "ResourceId";
		private const string UPDATE_INTERVAL_ELEMENT = "UpdateInterval";
		private const string PORT_ELEMENT = "Port";
		private const string USERNAME_ELEMENT = "Username";
		private const string PASSWORD_ELEMENT = "Password";

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(AsureDevice); } }

		[SettingsProperty(SettingsProperty.ePropertyType.PortId)]
		public int? Port { get; set; }

		public string Username { get; set; }
		public string Password { get; set; }

		public int ResourceId { get; set; }
		public long? UpdateInterval { get; set; }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			if (ResourceId != 0)
				writer.WriteElementString(RESOURCE_ID_ELEMENT, IcdXmlConvert.ToString(ResourceId));

			if (UpdateInterval != null)
				writer.WriteElementString(UPDATE_INTERVAL_ELEMENT, IcdXmlConvert.ToString((long)UpdateInterval));

			if (Port != null)
				writer.WriteElementString(PORT_ELEMENT, IcdXmlConvert.ToString((int)Port));

			if (!string.IsNullOrEmpty(Username))
				writer.WriteElementString(USERNAME_ELEMENT, Username);

			if (!string.IsNullOrEmpty(Password))
				writer.WriteElementString(PASSWORD_ELEMENT, Password);
		}

		/// <summary>
		/// Loads the settings from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI, XmlDeviceSettingsFactoryMethod(FACTORY_NAME)]
		public static AsureDeviceSettings FromXml(string xml)
		{
			int? port = XmlUtils.TryReadChildElementContentAsInt(xml, PORT_ELEMENT);
			int? resourceId = XmlUtils.TryReadChildElementContentAsInt(xml, RESOURCE_ID_ELEMENT);
			long? updateInterval = XmlUtils.TryReadChildElementContentAsLong(xml, UPDATE_INTERVAL_ELEMENT);
			string username = XmlUtils.TryReadChildElementContentAsString(xml, USERNAME_ELEMENT);
			string password = XmlUtils.TryReadChildElementContentAsString(xml, PASSWORD_ELEMENT);

			AsureDeviceSettings output = new AsureDeviceSettings
			{
				Port = port,
				UpdateInterval = updateInterval,
				Username = username,
				Password = password
			};

			if (resourceId != null)
				output.ResourceId = (int)resourceId;

			ParseXml(output, xml);
			return output;
		}
	}
}
