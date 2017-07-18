using System;
using ICD.Common.Properties;
using ICD.Connect.Settings.Attributes.Factories;

namespace ICD.Connect.Cameras.Panasonic
{
    public class PanasonicCameraDeviceSettings : AbstractCameraDeviceSettings
    {
        private const string FACTORY_NAME = "PanasonicCamera";
        /// <summary>
        /// Gets the originator factory name.
        /// </summary>
        public override string FactoryName { get { return FACTORY_NAME; } }

        /// <summary>
        /// Gets the type of the originator for this settings instance.
        /// </summary>
        public override Type OriginatorType { get { return typeof(PanasonicCameraDevice); } }

        [PublicAPI, XmlDeviceSettingsFactoryMethod(FACTORY_NAME)]
        public static PanasonicCameraDeviceSettings FromXml(string xml)
        {
            PanasonicCameraDeviceSettings output = new PanasonicCameraDeviceSettings();
            ParseXml(output, xml);
            return output;
        }
    }
}