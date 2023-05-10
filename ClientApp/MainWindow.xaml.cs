using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;

namespace ClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModel model;
        int counter = 0;
        DispatcherTimer timer;
        bool isPlaying;
        public MainWindow()
        {
            InitializeComponent();
            isPlaying = false;
            model = new ViewModel();
            this.DataContext = model;
            Button_Delete.IsEnabled = false;
            if (Directory.GetFiles("Songs").Length != 0)
            {
                string[] files = Directory.GetFiles(@"Songs", "*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    //Console.WriteLine(System.IO.Path.GetFileName(file));
                    model.AddSongs(new Song(System.IO.Path.GetFileNameWithoutExtension(file)));
                }
            }
            mediaElement.MediaOpened += MediaElement_MediaOpened;
            mediaElement.MediaEnded += MediaElement_MediaEnded;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
        }

        private void MediaElement_MediaEnded(object? sender, EventArgs e)
        {
            try
            {
                mediaElement.Position = TimeSpan.Zero;
                mediaElement.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void MediaElement_MediaOpened(object? sender, EventArgs e)
        {
            //slash.Content = @"\";
            //model.SetCurrentDuration(mediaElement.Position, ListMusic.SelectedItem as Song);
            //model.SetFullDuration(mediaElement.NaturalDuration.TimeSpan, ListMusic.SelectedItem as Song);
            //model.CurrentSongDuration = mediaElement.Position;
            //model.FullSongDuration = mediaElement.NaturalDuration.TimeSpan;
            try
            {
                model.SongDuration = string.Format("{0:mm\\:ss}/{1:mm\\:ss}", mediaElement.Position, mediaElement.NaturalDuration.TimeSpan);
                timer.Start();
                slider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            //durationLabel.Content = string.Format("{0:mm\\:ss}/{1:mm\\:ss}", mediaElement.Position, mediaElement.NaturalDuration.TimeSpan);
            //model.SetCurrentDuration(mediaElement.Position, ListMusic.SelectedItem as Song);
            //model.SetFullDuration(mediaElement.NaturalDuration.TimeSpan, ListMusic.SelectedItem as Song);
            //model.CurrentSongDuration = mediaElement.Position;
            //model.FullSongDuration = mediaElement.NaturalDuration.TimeSpan;
            try
            {
                model.SongDuration = string.Format("{0:mm\\:ss}/{1:mm\\:ss}", mediaElement.Position, mediaElement.NaturalDuration.TimeSpan);
                slider.Value = mediaElement.Position.TotalSeconds;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (ListMusic.SelectedItem != null)
            //{
            //    ListMusic.SelectedItem = null;
            //}
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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    File.Copy(openFileDialog.FileName, System.IO.Path.Combine("Songs", openFileDialog.SafeFileName));
                    model.AddSongs(new Song(System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName)));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                
            }
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
            //if (isPlaying)
            //{
            //    Task.Run(() =>
            //    {
            //        mediaElement.Dispatcher.Invoke(() =>
            //        {
            //            //mediaElement.Source = new Uri(System.IO.Path.Combine(hardcode, model.GetName(ListMusic.SelectedItem as Song) + ".mp3"));
            //            mediaElement.Pause();
            //        });
            //    });
            //}
            try
            {
                var selectedsong = ListMusic.SelectedItem as Song;
                if (ListMusic.SelectedItem != null)
                {
                    string path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Songs");
                    mediaElement.Source = new Uri(System.IO.Path.Combine(path, model.GetName(selectedsong) + ".mp3"));
                }
                if (ListMusic.SelectedItem != null && !isPlaying)
                {
                    //model.SelectedName = model.GetName(ListMusic.SelectedItem as Song);
                    
                    Button_Delete.IsEnabled = true;
                }
                else if (ListMusic.SelectedItem == null || isPlaying)
                {
                    Button_Delete.IsEnabled = false;
                    if (ListMusic.SelectedItem != null)
                    {
                        model.SelectedName = model.GetName(selectedsong);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Close();
            var selectedsong = ListMusic.SelectedItem as Song;
                    try
                    {
                slider.Value = 0;
                        model.SelectedName = "";
                        mediaElement.Position = TimeSpan.Zero;
                model.SongDuration = "00:00";
                        File.Delete(System.IO.Path.Combine("Songs", model.GetName(selectedsong) + ".mp3"));
                        model.ClearSong(selectedsong);
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.Message);
                    }
            
            
        }

        private void Button_Play_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ListMusic.SelectedItem != null)
                {
                    model.SelectedName = model.GetName(ListMusic.SelectedItem as Song);
                    //MessageBox.Show(System.IO.Path.Combine("Songs", model.GetName(ListMusic.SelectedItem as Song) + ".mp3"));
                    //mediaElement.Source = new Uri(System.IO.Path.Combine("Songs", model.GetName(ListMusic.SelectedItem as Song)));
                    //string hardcode = "C:\\Users\\User\\Desktop\\Нова папка\\MusicPlayer\\ClientApp\\bin\\Debug\\net6.0-windows\\Songs";
                    //string path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Songs");
                    var imageplay = new Image { Source = new BitmapImage(new Uri("/Images/play.png", UriKind.Relative)) };
                    var imagepause = new Image { Source = new BitmapImage(new Uri("/Images/pause.png", UriKind.Relative)) };
                    //mediaElement.Source = new Uri(System.IO.Path.Combine(hardcode, model.GetName(ListMusic.SelectedItem as Song) + ".mp3"));
                    //MessageBox.Show(System.IO.Path.Combine("Songs", model.GetName(ListMusic.SelectedItem as Song) + ".mp3"));
                    //return;

                    if (isPlaying == false)
                    {
                        Button_Play.Content = imagepause;
                        Button_Delete.IsEnabled = false;
                        isPlaying = true;
                        //mediaPlayer.Open(new Uri(@"Songs/" + model.GetName(ListMusic.SelectedItem as Song)));
                        //Task.Run(() =>
                        //{
                        //    mediaElement.Dispatcher.Invoke(() =>
                        //    {
                        //        //mediaElement.Source = new Uri(System.IO.Path.Combine(path, model.GetName(ListMusic.SelectedItem as Song) + ".mp3"));
                        //        mediaElement.Play();
                        //    });
                        //});

                        mediaElement.Play();

                        //mediaElement.Play();
                        //ListMusic.IsEnabled = false;
                        //ListMusic.IsHitTestVisible = false;
                    }
                    else
                    {
                        Button_Play.Content = imageplay;
                        isPlaying = false;
                        Button_Delete.IsEnabled = true;
                        //Task.Run(() =>
                        //{
                        //    mediaElement.Dispatcher.Invoke(() =>
                        //    {
                        //        //mediaElement.Source = new Uri(System.IO.Path.Combine(hardcode, model.GetName(ListMusic.SelectedItem as Song) + ".mp3"));
                        //        mediaElement.Pause();
                        //    });
                        //});

                        mediaElement.Pause();

                        //ListMusic.IsEnabled = true;
                        //ListMusic.IsHitTestVisible = true;
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaElement.Position = TimeSpan.FromSeconds(slider.Value);
        }
    }
    
}
