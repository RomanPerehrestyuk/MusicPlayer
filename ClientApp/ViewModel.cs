using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    [AddINotifyPropertyChangedInterface]
    class ViewModel
    {
        public string SongSize { get; set; }
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
        public void Clear()
        {
            songs.Clear();
        }
    }
}
