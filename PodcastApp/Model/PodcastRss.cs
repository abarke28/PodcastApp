using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PodcastApp.Model
{
    public class CData
    {
        public string ActualString { get; set; }
    }

    [XmlRoot(ElementName = "item")]
    public class Item
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }

        private string _pubDate;
        [XmlElement(ElementName = "pubDate")]
        public string PubDate
        {
            get { return _pubDate; }
            set
            {
                _pubDate = DateTime.Parse(value, CultureInfo.CurrentCulture).ToString();
                PublishedDate = DateTime.Parse(PubDate, CultureInfo.CurrentCulture);
            }
        }
        public DateTime PublishedDate { get; set; }

        [XmlElement(ElementName = "link")]
        public string Link { get; set; }

        [XmlElement(ElementName = "description")]
        public string Description { get; set; }
    }

    [XmlRoot(ElementName = "channel")]
    public class Channel
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }

        [XmlElement(ElementName = "description")]
        public string Description { get; set; }

        [XmlElement(ElementName ="image")]
        public Image Image { get; set; }

        [XmlElement(ElementName = "item")]
        public List<Item> Items { get; set; }
    }
    
    [XmlRoot(ElementName ="image")]
    public class Image
    {
        [XmlElement(ElementName ="url")]
        public string ImageUrl { get; set; }
    }

    [XmlRoot(ElementName = "rss")]
    public class PodcastRss
    {
        [XmlElement(ElementName = "channel")]
        public Channel Channel { get; set; }
    }
}
