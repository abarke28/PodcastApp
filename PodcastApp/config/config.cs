using PodcastApp.utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodcastApp.config
{
    public class Config
    {
        public static string AppDirectory { get; private set; }
        public static string AppConfigDirectory { get; private set; }
        public static string AppConfigFile { get; private set; } = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\config\config.xml";
        public static string AudioFilesDirectory { get; private set; }
        public static string PodcastThumbnailsDirectory { get; private set; }
        public static string AppVisualResourcesDirectory { get; private set; }
        public static string AppName { get; private set; } = "Poor Yorrick Podcasts";
        private Config()
        {

            AppDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\" + AppName;
            AppConfigDirectory = AppDirectory + @"\config";
            AppConfigFile = AppConfigDirectory + @"\config.xml";
            AudioFilesDirectory = AppDirectory + @"\audio";
            PodcastThumbnailsDirectory = AppDirectory + @"\thumbnails";
            AppVisualResourcesDirectory = AppDirectory + @"\images";
        }
        public static Config GetConfig()
        {
            if (!File.Exists(AppConfigFile))
            {
                return new Config();
            }
            return Serializer.DeserializeFromXmlFile<Config>(AppConfigDirectory + @"\config.xml");
        }
    }
}
