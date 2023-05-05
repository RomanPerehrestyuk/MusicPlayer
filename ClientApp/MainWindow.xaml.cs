using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ListMusic.SelectedItem != null)
            {
                ListMusic.SelectedItem = null;
            }
        }

        private void ListMusic_MouseEnter(object sender, MouseEventArgs e)
        {
            ListMusic.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#32e6b3"));
        }

        private void ListMusic_MouseLeave(object sender, MouseEventArgs e)
        {
            ListMusic.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34a8eb"));
        }
    }
    
}
