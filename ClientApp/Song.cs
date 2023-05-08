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
        public int CurrentDuration { get; set; }
        public int FullDuration { get; set; }
    }
}
