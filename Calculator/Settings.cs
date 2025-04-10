using System;
using System.IO;
using System.Xml.Serialization;

namespace Calculator
{
    public class Settings
    {
        public bool DigitGrouping { get; set; }
        public string Mode { get; set; }
        public int Base { get; set; }
        public bool AdvancedMode { get; set; }
        public static void SaveSettings(Settings settings)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            using (StreamWriter writer = new StreamWriter("settings.xml"))
            {
                serializer.Serialize(writer, settings);
            }
        }

        public static Settings LoadSettings()
        {
            if (File.Exists("settings.xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                using (StreamReader reader = new StreamReader("settings.xml"))
                {
                    return (Settings)serializer.Deserialize(reader);
                }
            }
            return new Settings { DigitGrouping = false, Mode = "Standard", Base = 10, AdvancedMode = false };
        }
    }
}
