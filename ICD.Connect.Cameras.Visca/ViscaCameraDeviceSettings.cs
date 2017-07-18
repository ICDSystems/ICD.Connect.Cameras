using System;
using ICD.Common.Properties;
using ICD.Connect.Settings.Attributes.Factories;

namespace ICD.Connect.Cameras.Visca
{
    public class ViscaCameraDeviceSettings : AbstractCameraDeviceSettings
    {
        private const string FACTORY_NAME = "ViscaCamera";
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
    }
}