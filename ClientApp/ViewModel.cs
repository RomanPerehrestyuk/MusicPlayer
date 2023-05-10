using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClientApp
{
    [AddINotifyPropertyChangedInterface]
    class ViewModel
    {
        public string SongSize { get; set; }
        public string SelectedName { get; set; }
        public string SongDuration { get; set; }
        public ViewModel()
        {
            songs = new ObservableCollection<Song>();
            SongSize = $"Songs Count: {Songs.Count()}";
        }
        ObservableCollection<Song> songs;
        public IEnumerable<Song> Songs => songs;
        public void AddSongs(Song song)
        {
            songs.Add(song);
            SongSize = $"Songs Count: {Songs.Count()}";
        }
        public void ClearAll()
        {
            songs.Clear();
        }

        public string GetName(Song song)
        {
               return song.Name;
        }

        //public void SetCurrentDuration(TimeSpan current_duration, Song song)
        //{
        //    song.CurrentDuration = current_duration;
        //}

        //public void SetFullDuration(TimeSpan full_duration, Song song)
        //{
        //    song.FullDuration = full_duration;
        //}

        public void ClearSong(Song song)
        {
                songs.Remove(song);
                SongSize = $"Songs Count: {Songs.Count()}";
        }
    }
}
