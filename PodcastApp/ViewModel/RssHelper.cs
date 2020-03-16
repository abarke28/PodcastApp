using PodcastApp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PodcastApp.ViewModel
{
    public class RssHelper
    {
        public static List<Item> GetEpisodes(string rssLink)
        {
            List<Item> posts = new List<Item>();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(PodcastRss));

            using (WebClient client = new WebClient())
            {
                string xml = Encoding.Default.GetString(client.DownloadData(rssLink));

                using (Stream reader = new MemoryStream(Encoding.Default.GetBytes(xml)))
                {
                    PodcastRss podcast = (PodcastRss)xmlSerializer.Deserialize(reader);

                    posts = podcast.Channel.Items;
                }
            }
            return posts;
        }

        public static PodcastRss GetInfo(string rsslink)
        {
            PodcastRss podcast = new PodcastRss();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(PodcastRss));

            using (WebClient client = new WebClient())
            {
                string xml = Encoding.Default.GetString(client.DownloadData(rsslink));

                using (Stream reader = new MemoryStream(Encoding.Default.GetBytes(xml)))
                {
                    podcast = (PodcastRss)xmlSerializer.Deserialize(reader);
                }
            }

            return podcast;
        }

        public static async Task<PodcastRss> GetInfoAsync(string rsslink)
        {
            PodcastRss podcast = new PodcastRss();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(PodcastRss));

            using (HttpClient httpClient = new HttpClient())
            {
                string xml = await Encoding.Default.GetString(httpClient.GetAsync(rsslink));

                using (Stream reader = new MemoryStream(Encoding.Default.GetBytes(xml)))
                {
                    podcast = (PodcastRss)xmlSerializer.Deserialize(reader);
                }
            }

            return podcast;
        }
    }
}
