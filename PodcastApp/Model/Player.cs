using PodcastApp.config;
using PodcastApp.View.AppResources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PodcastApp.Model
{
    public class Player : INotifyPropertyChanged
    {
        // Properties

        private MediaPlayer _player;

        private bool _isPlaying;
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                if (_isPlaying == value) return;
                _isPlaying = value;

                switch (_isPlaying)
                {
                    case true:
                        PlayPauseImageSource = AppResources.PAUSE_IMAGE;
                        break;
                    case false:
                        PlayPauseImageSource = AppResources.PLAY_IMAGE;
                        break;
                }

                OnPropertyChanged("IsPlaying");
            }
        }

        private bool _isMuted;
        public bool IsMuted
        {
            get { return _isMuted; }
            set
            {
                if (_isMuted == value) return;
                _isMuted = value;

                switch (_isMuted)
                {
                    case true:
                        _player.IsMuted = true;
                        AudioStateImageSource = AppResources.MUTED_IMAGE;
                        break;
                    case false:
                        _player.IsMuted = false;
                        AudioStateImageSource = AppResources.PLAYING_SOUND_IMAGE;
                        break;
                }

                OnPropertyChanged("IsMuted");
            }
        }

        private bool _mediaIsLoaded;
        public bool MediaIsLoaded
        {
            get { return _mediaIsLoaded; }
            set
            {
                if (_mediaIsLoaded == value) return;
                _mediaIsLoaded = value;

                System.Diagnostics.Debug.WriteLine("In MediaIsLoaded setter");
                OnPropertyChanged("MediaIsLoaded");
            }
        }

        private string _thumbnailSource;
        public string ThumbnailSource
        {
            get { return _thumbnailSource; }
            set 
            {
                if (_thumbnailSource == value) return;
                _thumbnailSource = value;
                OnPropertyChanged("ThumbnailSource");
            }
        }

        private string _playPauseImageSource;
        public string PlayPauseImageSource
        {
            get { return _playPauseImageSource; }
            set
            {
                if (_playPauseImageSource == value) return;
                _playPauseImageSource = value;
                OnPropertyChanged("PlayPauseImageSource");
            }
        }

        private string _podcastImageSource;
        public string PodcastImageSource
        {
            get { return _podcastImageSource; }
            set
            {
                if (_podcastImageSource == value) return;
                _podcastImageSource = value;
                OnPropertyChanged("PodcastImageSource");
            }
        }

        private string _replayImageSource;
        public string ReplayImageSource
        {
            get { return _replayImageSource; }
            set
            {
                if (_replayImageSource == value) return;
                _replayImageSource = value;
                OnPropertyChanged("ReplayImageSource");
            }
        }

        private string _forwardImageSource;
        public string ForwardImageSource
        {
            get { return _forwardImageSource; }
            set
            {
                if (_forwardImageSource == value) return;
                _forwardImageSource = value;
                OnPropertyChanged("ForwardImageSource");
            }
        }

        private string _audioStateImageSource;
        public string AudioStateImageSource
        {
            get { return _audioStateImageSource; }
            set
            {
                if (_audioStateImageSource == value) return;
                _audioStateImageSource = value;
                OnPropertyChanged("AudioStateImageSource");
            }
        }

        private string _audioSource;
        public string AudioSource
        {
            get { return _audioSource; }
            set
            {
                if (_audioSource == value) return;
                _audioSource = value;
                OnPropertyChanged("AudioSource");
            }
        }

        private SyndicationItem _playingEpisode;
        public SyndicationItem PlayingEpisode
        {
            get { return _playingEpisode; }
            set
            {
                if (_playingEpisode == value) return;
                _playingEpisode = value;
                OnPropertyChanged("PlayingEpisode");
            }
        }

        // Constructors

        public Player()
        {
            _player = new MediaPlayer();
            IsPlaying = false;
            MediaIsLoaded = false;
            ThumbnailSource = AppResources.BLANK_IMAGE;
            PlayPauseImageSource = AppResources.PLAY_IMAGE;
            ReplayImageSource = AppResources.REWIND_10_IMAGE;
            ForwardImageSource = AppResources.FORWARD_10_IMAGE;
            AudioStateImageSource = AppResources.PLAYING_SOUND_IMAGE;
        }

        // Methods

        public async void PlayAudio()
        {
            // Summary
            //
            // Fetch RSS supplied audio file. Download locally then play. Resolves Uri and removes illegal directory characters before playing.
            // Set MediaIsLoaded and IsPlaying flags appropriately to control player image strings for binding with UI

            AudioSource = ResolveUri(PlayingEpisode);

            string resolvedTitle = ResolveTitle(PlayingEpisode);

            // Fetch audio library from Config file.
            string filePath = Config.GetConfig().AudioFilesDirectory + @"\";

            // Check if file has already been downloaded
            if (!File.Exists(filePath + resolvedTitle + @".mp3"))
            {
                using (WebClient webClient = new WebClient())
                {
                    await webClient.DownloadFileTaskAsync(AudioSource, filePath + resolvedTitle + @".mp3");
                }
            }

            if (_player == null) _player = new MediaPlayer();

            _player.Open(new Uri(filePath + resolvedTitle + @".mp3"));

            System.Diagnostics.Debug.WriteLine("MediaIsLoaded is True - Player.PlayAudio()");
            MediaIsLoaded = true;

            _player.Position = TimeSpan.Zero;

            System.Diagnostics.Debug.WriteLine("Playback of " + PlayingEpisode.Title.Text + " has stated");
            _player.Play();

            IsPlaying = true;
        }
        public void PauseAudio()
        {
            //Summary
            //
            //Error handling handled in VM caller method

            _player.Pause();
            IsPlaying = false;
        }
        public void ResumeAudio()
        {
            // Summary
            //
            // Error handling handled in VM caller

            _player.Play();
            IsPlaying = true;
        }
        public void StopAudio()
        {
            // Summary
            //
            // Stop and close media on player before, reset appropriate image sources

            _player.Stop();
            _player.Close();
            IsPlaying = false;
            System.Diagnostics.Debug.WriteLine("MediaIsLoaded is False - Player.StopAudio()");
            MediaIsLoaded = false;
            ThumbnailSource = AppResources.BLANK_IMAGE;
        }
        public void FastForwardAudio()
        {
            // Summary
            //
            // Check if we are within jump time from end of audio. If so, end playback and set flags appropriately.
            // Else advance 10s. Sudden skip is jarring so pause for 350ms
            
            _player.Pause();
            System.Threading.Thread.Sleep(350);

            if (_player.Position > _player.NaturalDuration - TimeSpan.FromSeconds(10))
            {
                _player.Stop();
                _player.Close();
                IsPlaying = false;
                MediaIsLoaded = false;
                ThumbnailSource = AppResources.BLANK_IMAGE;
            }
           
            _player.Position += TimeSpan.FromSeconds(10);
            _player.Play();
        }
        public void RewindAudio()
        {
            // Summary
            //
            // Check we are not within skip time from start. If we are, go to position 0. 
            // Else, rewind 10s. Sudden rewind is audibly jarring. Pause for a moment

            if (_player.Position < TimeSpan.FromSeconds(10))
            {
                _player.Pause();
                System.Threading.Thread.Sleep(350);
                _player.Position = TimeSpan.Zero;
                _player.Play();
            }

            else
            {
                _player.Pause();
                System.Threading.Thread.Sleep(500);
                _player.Position -= TimeSpan.FromSeconds(10);
                _player.Play();
            }
        }
        public void MuteAudio()
        {
            IsMuted = true;
        }
        public void UnmuteAudio()
        {
            IsMuted = false;
        }
        private static string ResolveUri(SyndicationItem item)
        {
            string resolvedUri = null;

            var uri = item.Links.Where(l => l.RelationshipType == "enclosure").First().Uri;

            if (uri.Query != null)
            {
                resolvedUri = uri.Scheme + @"://" + uri.Authority + uri.AbsolutePath;
            }

            else
            {
                resolvedUri = uri.OriginalString;
            }

            return resolvedUri;
        }
        private static string ResolveTitle(SyndicationItem item)
        {
            // Summary
            //
            // Parse title for invalid filename characters and remove them

            string invalidCharacters = new string(Path.GetInvalidFileNameChars());
            string resolvedTitle = item.Title.Text;

            foreach (char c in invalidCharacters)
            {
                resolvedTitle = resolvedTitle.Replace(c.ToString(), "");
            }

            return resolvedTitle;
        }

        // Events & Handlers

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            if (property == "MediaIsLoaded") System.Diagnostics.Debug.WriteLine("In Player.OnPropertyChanged for MediaIsLoaded");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}