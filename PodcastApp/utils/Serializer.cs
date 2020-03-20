using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PodcastApp.utils
{
    public static class Serializer
    {
        public static void SerializeToXmlFile<T>(string filePath, object file)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            using (XmlWriter xmlWriter = XmlWriter.Create(filePath))
            {
                xmlSerializer.Serialize(xmlWriter, file);
            }
        }

        public static T DeserializeFromXmlFile<T>(string filePath)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            using (XmlReader xmlReader = XmlReader.Create(filePath))
            {
                return (T)xmlSerializer.Deserialize(xmlReader);
            }
        }
    }
}
