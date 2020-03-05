using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodcastApp.Model
{
    public class Player : INotifyPropertyChanged
    {
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
            PlayPauseImageSource = @"c:\Users\Owner\source\repos\PodcastApp\PodcastApp\View\AppResources\Play.png";
            ReplayImageSource = @"c:\Users\Owner\source\repos\PodcastApp\PodcastApp\View\AppResources\Replay10.png";
            ForwardImageSource = @"c:\Users\Owner\source\repos\PodcastApp\PodcastApp\View\AppResources\Forward10.png";
            AudioStateImageSource = @"c:\Users\Owner\source\repos\PodcastApp\PodcastApp\View\AppResources\Sound.png";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
