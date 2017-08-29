using System;
using ICD.Common.Properties;
using ICD.Connect.Settings.Attributes.Factories;
using ICD.Common.Utils.Xml;

namespace ICD.Connect.Cameras.Visca
{
    public sealed class ViscaCameraDeviceSettings : AbstractCameraDeviceSettings
    {
        private const string FACTORY_NAME = "ViscaCamera";
        private const string PORT_ELEMENT = "Port";
        /// <summary>
        /// Gets the originator factory name.
        /// </summary>
        public override string FactoryName { get { return FACTORY_NAME; } }

        /// <summary>
        /// Gets the type of the originator for this settings instance.
        /// </summary>
        public override Type OriginatorType { get { return typeof(ViscaCameraDevice); } }

        [PublicAPI, XmlDeviceSettingsFactoryMethod(FACTORY_NAME)]
        public static ViscaCameraDeviceSettings FromXml(string xml)
        {
            ViscaCameraDeviceSettings output = new ViscaCameraDeviceSettings();
            ParseXml(output, xml);
            return output;
        }

        /// <summary>
        /// Write settings elements to xml.
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteElements(IcdXmlTextWriter writer)
        {
            base.WriteElements(writer);

            if (Port != null)
                writer.WriteElementString(PORT_ELEMENT, IcdXmlConvert.ToString((int)Port));
        }
    }
}