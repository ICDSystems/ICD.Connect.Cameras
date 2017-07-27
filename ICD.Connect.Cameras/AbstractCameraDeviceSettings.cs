using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using ICD.Connect.Settings;
using ICD.Common.Utils.Xml;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Cameras
{
    public abstract class AbstractCameraDeviceSettings : AbstractDeviceSettings, ICameraDeviceSettings
    {
        private const string PORT_ELEMENT = "Port";

        [SettingsProperty(SettingsProperty.ePropertyType.PortId)]
        public int? Port { get; set; }

        /// <summary>
        /// Writes property elements to xml.
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteElements(IcdXmlTextWriter writer)
        {
            base.WriteElements(writer);

            writer.WriteElementString(PORT_ELEMENT, IcdXmlConvert.ToString(Port));
        }

        /// <summary>
        /// Parses the xml and applies the properties to the instance.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="xml"></param>
        protected static void ParseXml(AbstractCameraDeviceSettings instance, string xml)
        {
            instance.Port = XmlUtils.TryReadChildElementContentAsInt(xml, PORT_ELEMENT);

            AbstractDeviceSettings.ParseXml(instance, xml);
        }
    }
}