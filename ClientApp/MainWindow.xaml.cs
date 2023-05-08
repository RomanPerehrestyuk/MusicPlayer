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
using System.Windows.Media.Animation;
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
        ViewModel model;
        int counter = 0;
        public MainWindow()
        {
            InitializeComponent();
            model = new ViewModel();
            this.DataContext = model;
            Button_Delete.IsEnabled = false;
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
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

        private void Button_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_RollUp_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            model.AddSongs(new Song("Hello"));
        }

        private void logo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            counter++;
            if (counter == 5)
            {
                RotateTransform rotateTransform = new RotateTransform();
                logo.RenderTransform = rotateTransform;

                DoubleAnimation animation = new DoubleAnimation();
                animation.From = 0;
                animation.To = 360;
                animation.Duration = TimeSpan.FromSeconds(2);
                animation.EasingFunction = new BackEase();

                rotateTransform.BeginAnimation(RotateTransform.AngleProperty, animation);
                counter = 0;
            }
        }

        private void ListMusic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ListMusic.SelectedItem != null)
            {
                Button_Delete.IsEnabled = true;
            }
            else
            {
                Button_Delete.IsEnabled = false;
            }
        }
    }
    
}
