using PodcastApp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PodcastApp.ViewModel
{
    public static class RssHelper
    {
        public static List<Item> GetEpisodes(string rssLink)
        {
            // Obsolete. Using Async method.

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
        public static async Task<List<Item>> GetEpisodesAsync(string rssLink)
        {
            // Summary
            //
            // Fetches list of Episodes from supplied Podcast.
            // Download RSS xml from web. Deserialize as per attributes in PodcastRSS.cs
            // Using XmlSerialer and XmlReader classes.

            //TODO: Add more error handling - not all podcasts implement RSS standard the same.

            List<Item> posts = new List<Item>();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(PodcastRss));

            using (HttpClient client = new HttpClient())
            {
                string xml = await client.GetStringAsync(new Uri(rssLink)).ConfigureAwait(true);

                XmlReader xmlReader = XmlReader.Create(new StringReader(xml));

                xmlReader.Read();

                PodcastRss podcast = (PodcastRss)xmlSerializer.Deserialize(xmlReader);

                posts = podcast.Channel.Items;

                xmlReader.Dispose();
            }

            return posts;
        }
        public static PodcastRss GetInfo(string rsslink)
        {
            // Obsolete. Using async method

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
            // Summary
            // 
            // Gets Podcast Info from provided RSS link. Deserialized with XmlDeserializer
            
            //TODO: Add more error handling.

            PodcastRss podcast = new PodcastRss();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(PodcastRss));

            using (HttpClient httpClient = new HttpClient())
            {
                string xml = await httpClient.GetStringAsync(new Uri(rsslink)).ConfigureAwait(false);

                using (Stream reader = new MemoryStream(Encoding.Default.GetBytes(xml)))
                {
                    podcast = (PodcastRss)xmlSerializer.Deserialize(reader);
                }
            }

            return podcast;
        }
        public static SyndicationFeed GetFeed(string rssLink)
        {
            // Summary
            //
            // Fetches RSS using System.ServiceModel.Syndication

            XmlReader xmlReader = XmlReader.Create(rssLink);

            SyndicationFeed syndicationFeed = SyndicationFeed.Load(xmlReader);

            xmlReader.Close();

            return syndicationFeed;
        }
    }
}
