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
        public string AppDirectory { get; set; }
        public string AppConfigDirectory { get; set; }
        public static string AppConfigFile { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\config\config.xml";
        public string AudioFilesDirectory { get; set; }
        public string PodcastThumbnailsDirectory { get; set; }
        public string AppVisualResourcesDirectory { get; set; }
        public string AppName { get; set; } = "Poor Yorrick Podcasts";
        private Config()
        {
            // Summary
            //
            // Private constructor that would only be called if no config is already present
            // therefore, create appropriate directories

            AppDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\" + AppName;
            Directory.CreateDirectory(AppDirectory);

            AppConfigDirectory = AppDirectory + @"\config";
            AppConfigFile = AppConfigDirectory + @"\config.xml";
            Directory.CreateDirectory(AppConfigDirectory);

            AudioFilesDirectory = AppDirectory + @"\audio";
            Directory.CreateDirectory(AudioFilesDirectory);

            PodcastThumbnailsDirectory = AppDirectory + @"\thumbnails";
            Directory.CreateDirectory(PodcastThumbnailsDirectory);

            AppVisualResourcesDirectory = AppDirectory + @"\images";
            Directory.CreateDirectory(AppVisualResourcesDirectory);
        }
        public static Config GetConfig()
        {
            // Summary
            //
            // Check if config file already exists, if so returns it. Else, call private
            // constructor to build the directories and config file

            if (!File.Exists(AppConfigFile))
            {
                Config config = new Config();
                Serializer.SerializeToXmlFile<Config>(AppConfigFile, config);
            }

            return Serializer.DeserializeFromXmlFile<Config>(AppConfigFile) ;
        }
    }
}
