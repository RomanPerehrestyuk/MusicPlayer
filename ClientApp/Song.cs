using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    [AddINotifyPropertyChangedInterface]
    public class Song
    {
        public Song(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public TimeSpan CurrentDuration { get; set; }
        public TimeSpan FullDuration { get; set; }
    }
}
