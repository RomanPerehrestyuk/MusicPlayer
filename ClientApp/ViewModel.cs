using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ClientApp
{
    [AddINotifyPropertyChangedInterface]
    class ViewModel
    {
        public string SongSize { get; set; }
        public string SelectedName { get; set; }
        public string SongDuration { get; set; }
        public string VolumeOfMusic { get; set; }
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
            SongSize = $"Songs Count: {Songs.Count()}";
        }
        public string GetName(Song song)
        {
            return song.Name;
        }
        public ObservableCollection<Song> GetSongs()
        {
            return songs;
        }
        public void ClearSong(Song song)
        {
            songs.Remove(song);
            SongSize = $"Songs Count: {Songs.Count()}";
        }
    }
}