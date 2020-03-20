﻿using PodcastApp.View.AppResources;
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

        private bool _mediaIsLoaded;
        public bool MediaIsLoaded
        {
            get { return _mediaIsLoaded; }
            set
            {
                if (_mediaIsLoaded == value) return;
                _mediaIsLoaded = value;
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

        public Player()
        {
            IsPlaying = false;
            MediaIsLoaded = false;
            ThumbnailSource = AppResources.BLANK_IMAGE;
            PlayPauseImageSource = AppResources.PLAY_IMAGE;
            ReplayImageSource = AppResources.REWIND_10_IMAGE;
            ForwardImageSource = AppResources.FORWARD_10_IMAGE;
            AudioStateImageSource = AppResources.PLAYING_SOUND_IMAGE;
        }

        public async void PlayAudio()
        {
            // Summary
            //
            // Fetch RSS supplied audio file. Download locally then play. Explore streaming direct to buffer. Small performance uptick.
            // Set MediaIsLoaded and IsPlaying flags appropriately to control player image strings for binding with UI

            AudioSource = PlayingEpisode.Links.First().Uri.OriginalString;

            System.Diagnostics.Debug.WriteLine("Retrieved Audio URI " + AudioSource.ToString());

            //AudioSource = @"https://dts.podtrac.com/redirect.mp3/media.blubrry.com/99percentinvisible/dovetail.prxu.org/96/0a4c4316-2d21-4e3b-82ba-d35f8b74aa3f/393_Map_Quest_pt01.mp3";

            using (WebClient webClient = new WebClient())
            {
                await webClient.DownloadFileTaskAsync(AudioSource, @"c:\Users\owner\desktop\" + PlayingEpisode.Title + @".mp3");
            }

            if (_player == null) _player = new MediaPlayer();

            //_player.Open(new Uri(@"c:\Users\Owner\desktop\testing.mp3"));

            _player.Open(new Uri(AudioSource));
            MediaIsLoaded = true;

            System.Diagnostics.Debug.WriteLine("MediaIsLoaded = {0}", MediaIsLoaded);

            _player.Position = TimeSpan.Zero;

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
            System.Diagnostics.Debug.WriteLine("Playback Paused");
        }
        public void ResumeAudio()
        {
            // Summary
            //
            // Error handling handled in VM caller

            _player.Play();
            IsPlaying = true;
            System.Diagnostics.Debug.WriteLine("Playback Resumed");
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}