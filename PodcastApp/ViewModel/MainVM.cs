using PodcastApp.Model;
using PodcastApp.View;
using PodcastApp.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PodcastApp.ViewModel
{
    public class MainVM : INotifyPropertyChanged
    {
        private ObservableCollection<Podcast> _podcasts;
        public ObservableCollection<Podcast> Podcasts
        {
            get { return _podcasts; }
            set
            {
                if (_podcasts == value) return;
                _podcasts = value;
                OnPropertyChanged("Podcasts");
            }
        }

        private ObservableCollection<Item> _episodes;
        public ObservableCollection<Item> Episodes
        {
            get { return _episodes; }
            set
            {
                if (_episodes == value) return;
                _episodes = value;
                OnPropertyChanged("Episodes");
            }
        }

        private Player _player;
        public Player Player
        {
            get { return _player; }
            set
            {
                if (_player == value) return;
                _player = value;
                OnPropertyChanged("Player");
            }
        }

        private Podcast _selectedPodcast;
        public Podcast SelectedPodcast
        {
            get { return _selectedPodcast; }
            set 
            {
                if (_selectedPodcast == value) return;
                _selectedPodcast = value;
                OnPropertyChanged("SelectedPodcast");
                ReadEpisodes(SelectedPodcast.RssLink);
            }
        }

        private Item _selectedEpisode;
        public Item SelectedEpisode
        {
            get { return _selectedEpisode; }
            set
            {
                if (_selectedEpisode == value) return;
                _selectedEpisode = value;
                OnPropertyChanged("SelectedEpisode");
            }
        }
        public ICommand ExitCommand { get; set; }
        public ICommand NewPodcastCommand { get; set; }
        public ICommand PlayEpisodeCommand { get; set; }

        public MainVM()
        {
            Podcasts = new ObservableCollection<Podcast>();
            Episodes = new ObservableCollection<Item>();
            Player = new Player();

            InstantiateCommands();
            ReadPodcasts();
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
            ExitCommand = new BaseCommand(x => true, x => ExitApplication());
            NewPodcastCommand = new BaseCommand(x => true, x => SubscribePodcast());
            PlayEpisodeCommand = new BaseCommand(e => CanPlayEpisode(e as Item), e => PlayEpisode(e as Item));
        }
        public void ExitApplication()
        {
            Application.Current.Shutdown();
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
        public void PlayEpisode(Item episode)
        {
            Player.PlayingEpisode = episode;
            Console.WriteLine("Entered Play Episode");
            Player.PlayAudio();
        }
        public bool CanPlayEpisode(Item episode)
        {
            return (episode.Title == SelectedEpisode.Title);
        }
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;

    }
}
