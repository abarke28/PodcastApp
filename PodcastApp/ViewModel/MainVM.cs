using PodcastApp.config;
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
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PodcastApp.ViewModel
{
    public class MainVM : INotifyPropertyChanged
    {
        // Properties

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

        private ObservableCollection<SyndicationItem> _episodes;
        public ObservableCollection<SyndicationItem> Episodes
        {
            get { return _episodes; }
            set
            {
                if (_episodes == value) return;
                _episodes = value;
                OnPropertyChanged("Episodes");
            }
        }

        private bool _episodesLoading;
        public bool EpisodesLoading
        {
            get { return _episodesLoading; }
            set
            {
                if (_episodesLoading == value) return;
                _episodesLoading = value;
                OnPropertyChanged("EpisodesLoading");
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
                Episodes.Clear();
                ReadEpisodesFromFeedAsync(SelectedPodcast.RssLink);
            }
        }

        private SyndicationItem _selectedEpisode;
        public SyndicationItem SelectedEpisode
        {
            get { return _selectedEpisode; }
            set
            {
                if (_selectedEpisode == value) return;
                _selectedEpisode = value;
                OnPropertyChanged("SelectedEpisode");
            }
        }

        // Commands

        public ICommand ExitCommand { get; set; }
        public ICommand NewPodcastCommand { get; set; }
        public ICommand PlayEpisodeCommand { get; set; }
        public ICommand PauseResumeEpisodeCommand { get; set; }
        public ICommand RewindEpisodeCommand { get; set; }
        public ICommand ForwardEpisodeCommand { get; set; }
        public ICommand MuteUnmuteEpisodeCommand { get; set; }
        public ICommand ClearDownloadsCommand { get; set; }

        // Constructors

        public MainVM()
        {
            Podcasts = new ObservableCollection<Podcast>();
            Episodes = new ObservableCollection<SyndicationItem>();
            Player = new Player();

            InstantiateCommands();
            ReadPodcasts();
        }

        // Methods 

        public async void ReadEpisodesFromFeedAsync(string rssLink)
        {
            //Summary
            //
            // Fetches episodes asynchronously using RssHelper.GetInfoAsync

            EpisodesLoading = true;

            var feed = await RssHelper.GetFeedAsync(rssLink).ConfigureAwait(true);

            EpisodesLoading = false;

            foreach(SyndicationItem episode in feed.Items)
            {
                Episodes.Add(episode);
            }
        }
        public void ReadEpisodesFromFeed(string rssLink)
        {
            // Summary
            //
            // Fetches episodes synchronously from RSS using System.ServiceModel.Syndication

            Episodes.Clear();

            var episodes = RssHelper.GetFeed(rssLink).Items;

            foreach (SyndicationItem episode in episodes)
            {
                Episodes.Add(episode);
            }
        }
        public void ReadPodcasts()
        {
            // Summary
            // Fetch list of Subscribed Podcasts from MS SQL Database. Utilizing utilities in DatabaseHelper.cs

            Podcasts.Clear();

            var podcasts = DatabaseHelper.GetPodcasts().OrderBy(p => p.Title);

            foreach (Podcast podcast in podcasts)
            {
                Podcasts.Add(podcast);
            }
        }
        public void InstantiateCommands()
        {
            // Summary
            //
            // Instantiate all commands for VM. x = nothing, e = episode, b = boolean predicate

            ExitCommand = new BaseCommand(x => true, x => ExitApplication());
            NewPodcastCommand = new BaseCommand(x => true, x => SubscribePodcast());
            PlayEpisodeCommand = new BaseCommand(e => true, e => PlayEpisode(e as SyndicationItem));
            PauseResumeEpisodeCommand = new BaseCommand(x => true, x => PauseResumeEpisode());
            RewindEpisodeCommand = new BaseCommand(x => true, x => RewindEpisode());
            ForwardEpisodeCommand = new BaseCommand(x => true, x => FastForwardEpisode());
            MuteUnmuteEpisodeCommand = new BaseCommand(x => true, x => MuteUnmuteEpisode());
            ClearDownloadsCommand = new BaseCommand(x => true, x => ClearDownloadedEpisodes());
        }
        public void ExitApplication()
        {
            Application.Current.Shutdown();
        }
        public void SubscribePodcast()
        {
            // Summary
            //
            // Gets RSS Link from user. Fetches necessary info from RSS link, deserializes the XML, then inserts into DB.
            // Downloads thumbnail of the podcast to store locally. 

            string rssLink = Prompt.ShowDialog("Podcast RSS Link", "Subscribe to New Podcast");

            if (String.IsNullOrEmpty(rssLink))
            {
                throw new ArgumentException("Invalid RSS feed");
            }

            Podcast podcast = new Podcast();
            SyndicationFeed syndicationFeed = RssHelper.GetFeed(rssLink);

            podcast.RssLink = rssLink;
            podcast.Title = syndicationFeed.Title.Text;
            podcast.ThumbnailFileUrl = syndicationFeed.ImageUrl.OriginalString;

            using (WebClient client = new WebClient())
            {
                string thumbnailDirectory = Config.GetConfig().PodcastThumbnailsDirectory + @"\";

                podcast.ThumbnailFileLocation = thumbnailDirectory + podcast.Title + ".png";

                client.DownloadFile(new Uri(podcast.ThumbnailFileUrl), podcast.ThumbnailFileLocation);
            }

            DatabaseHelper.InsertPodcast(podcast);

            ReadPodcasts();
        }
        public void PlayEpisode(SyndicationItem episode)
        {
            // Summary
            //
            // Check if Player is already playing. If so, stop current playback. 
            // Apply selected episode thumbnail to Player bar, then pass episode to the Player for audio playback.

            if (Player.PlayingEpisode != null)
            {
                Player.StopAudio();
            }

            Player.PlayingEpisode = episode;
            Player.ThumbnailSource = SelectedPodcast.ThumbnailFileLocation;
            Player.PlayAudio();
        }
        public bool CanPauseResumeEpisode()
        {
            // Summary
            //
            // Predicate for PauseResumeEpisodeCommand

            return Player.MediaIsLoaded;
        }
        public void PauseResumeEpisode()
        {
            // Summary
            //
            // Examine Player flags and call appropriate play or pause function.
            // Player.cs handles images for playback bar.

            switch (Player.IsPlaying)
            {
                case true:
                    Player.PauseAudio();
                    break;
                case false:
                    Player.ResumeAudio();
                    break;
            }
        }
        public void FastForwardEpisode()
        {
            Player.FastForwardAudio();
        }
        public void RewindEpisode()
        {
            Player.RewindAudio();
        }
        public void MuteUnmuteEpisode()
        {
            switch (Player.IsMuted)
            {
                case true:
                    Player.UnmuteAudio();
                    break;
                case false:
                    Player.MuteAudio();
                    break;
            }
        }
        public void ClearDownloadedEpisodes()
        {
            // Summary
            //
            // Delete all downloaded episodes. Try each file, alert if any were unable to be deleted.

            DirectoryInfo directoryInfo = new DirectoryInfo(Config.GetConfig().AudioFilesDirectory);
            int numDeletedFiles = 0;
            int numTotalFiles = directoryInfo.GetFiles().Length;
            
            foreach (FileInfo fileInfo in directoryInfo.GetFiles())
            {
                try
                {
                    fileInfo.Delete();
                    numDeletedFiles++;
                }
                catch (Exception)
                {
                    System.Diagnostics.Debug.WriteLine("Could not delete file {0}", fileInfo.Name);
                    MessageBox.Show("Could not delete file " + fileInfo.Name, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            System.Diagnostics.Debug.WriteLine("Deleted {0}/{1} files", numDeletedFiles, numTotalFiles);
        }

        // Events & Handlers

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}