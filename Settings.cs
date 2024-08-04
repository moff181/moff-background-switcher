using System.IO;
using System.Xml.Linq;

namespace MoffBackgroundSwitcher
{
    public class Settings
    {
        private const string FileName = "Settings.xml";

        public string BackgroundDirectory { get; set; }

        public void Save()
        {
            var doc = new XElement(
                "Settings",
                new XElement("BackgroundDirectory", BackgroundDirectory));
            doc.Save(FileName);
        }

        public static Settings Load()
        {
            const string defaultBackgroundDirectory = "SET PATH";

            if(!File.Exists(FileName))
            {
                var settings = new Settings
                {
                    BackgroundDirectory = defaultBackgroundDirectory,
                };
                settings.Save();
                return settings;
            }

            var doc = XElement.Load(FileName);
            return new Settings
            {
                BackgroundDirectory = doc.Element("BackgroundDirectory")?.Value ?? defaultBackgroundDirectory,
            };
        }
    }
}
