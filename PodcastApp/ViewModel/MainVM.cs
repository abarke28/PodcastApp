using PodcastApp.Model;
using PodcastApp.View;
using PodcastApp.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PodcastApp.ViewModel
{
    public class MainVM
    {
        public ObservableCollection<Podcast> Podcasts { get; set; }
        public ObservableCollection<Item> Episodes { get; set; }

        private Podcast _selectedPodcast;
        public Podcast SelectedPodcast
        {
            get { return _selectedPodcast; }
            set 
            { 
                _selectedPodcast = value;
                ReadEpisodes(SelectedPodcast.RssLink);
            }
        }

        private Item _selectedEpisode;
        public Item SelectedEpisode
        {
            get { return _selectedEpisode; }
            set { _selectedEpisode = value; }
        }

        public BaseCommand NewPodcastCommand { get; set; }
        public BaseCommand ExitCommand { get; set; }
        public MainVM()
        {
            Podcasts = new ObservableCollection<Podcast>();
            Episodes = new ObservableCollection<Item>();

            InstantiateCommands();
            ReadPodcasts();

            //Some rss' for now
            //https://rss.art19.com/monday-morning-podcast
            //http://wakingup.libsyn.com/rss
            //https://audioboom.com/channels/4940872.rss

            //SubscribePodcast();
        }
        public void ReadPodcasts()
        {
            Podcasts.Clear();
            
            var podcasts = DatabaseHelper.GetPodcasts();

            foreach(Podcast podcast in podcasts)
            {
                Podcasts.Add(podcast);
            }
        }
        public void SubscribePodcast()
        {
            string rssLink = Prompt.ShowDialog("Podcast RSS Link", "Subscribe to New Podcast");

            if (rssLink == "") 
            {
                throw new ArgumentException("Invalid RSS feed");
            } 
            
            Podcast podcast = new Podcast();
            PodcastRss podcastRss = RssHelper.GetInfo(rssLink);

            podcast.RssLink = rssLink;
            podcast.Title = podcastRss.Channel.Title;
            podcast.ThumbnailFileUrl = podcastRss.Channel.Image.ImageUrl;

            using (WebClient client = new WebClient())
            {
                string workingDirectory = Environment.CurrentDirectory;
                string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
                //string imageDirectory = 

                podcast.ThumbnailFileLocation = projectDirectory + @"\thumbnails\" + podcast.Title + ".png";

                Directory.CreateDirectory(projectDirectory + @"\thumbnails");
                client.DownloadFile(new Uri(podcast.ThumbnailFileUrl), podcast.ThumbnailFileLocation);
            }

            DatabaseHelper.InsertPodcast(podcast);

            ReadPodcasts();
        }
        public void ReadEpisodes(string rssLink)
        {
            Episodes.Clear();

            var episodes = RssHelper.GetEpisodes(rssLink);

            foreach (var episode in episodes)
            {
                Episodes.Add(episode);
            }
        }
        public void InstantiateCommands()
        {
         //   ExitCommand = new BaseCommand(x => true, ExitApplication);
        }
        public void ExitApplication()
        {
            Application.Current.Shutdown();
        }
    }
}
