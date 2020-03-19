using PodcastApp.View.AppResources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Http;
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
                OnPropertyChanged("IsPlaying");
            }
        }

        private string _thumbnailSource;
        public string ThumbnailSource
        {
            get { return _thumbnailSource; }
            set { _thumbnailSource = value; }
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

        private Item _playingEpisode;
        public Item PlayingEpisode
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
            PlayPauseImageSource = AppResources.PLAY_IMAGE;
            ReplayImageSource = AppResources.REWIND_10_IMAGE;
            ForwardImageSource = AppResources.FORWARD_10_IMAGE;
            AudioStateImageSource = AppResources.PLAYING_SOUND_IMAGE;
        }

        public void PlayAudio()
        {
            AudioSource = PlayingEpisode.Link;
            IsPlaying = true;
            PlayPauseImageSource = AppResources.PAUSE_IMAGE;

            AudioSource = @"https://dts.podtrac.com/redirect.mp3/media.blubrry.com/99percentinvisible/dovetail.prxu.org/96/0a4c4316-2d21-4e3b-82ba-d35f8b74aa3f/393_Map_Quest_pt01.mp3";

            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(AudioSource, @"c:\Users\owner\desktop\testing.mp3");
            }

            if (_player == null)
            {
                _player = new MediaPlayer();
            }

            _player.Open(new Uri(@"c:\Users\Owner\desktop\testing.mp3"));
            _player.Play();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
