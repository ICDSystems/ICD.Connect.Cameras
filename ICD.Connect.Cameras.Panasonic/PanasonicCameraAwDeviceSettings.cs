using System;
using ICD.Common.Properties;
using ICD.Connect.Settings.Attributes.Factories;

namespace ICD.Connect.Cameras.Panasonic
{
    public class PanasonicCameraAwDeviceSettings : AbstractCameraDeviceSettings
    {
        private const string FACTORY_NAME = "PanasonicCamera";
        /// <summary>
        /// Gets the originator factory name.
        /// </summary>
        public override string FactoryName { get { return FACTORY_NAME; } }

        /// <summary>
        /// Gets the type of the originator for this settings instance.
        /// </summary>
        public override Type OriginatorType { get { return typeof(PanasonicCameraAwDevice); } }

        [PublicAPI, XmlDeviceSettingsFactoryMethod(FACTORY_NAME)]
        public static PanasonicCameraAwDeviceSettings FromXml(string xml)
        {
            PanasonicCameraAwDeviceSettings output = new PanasonicCameraAwDeviceSettings();
            ParseXml(output, xml);
            return output;
        }
    }
}