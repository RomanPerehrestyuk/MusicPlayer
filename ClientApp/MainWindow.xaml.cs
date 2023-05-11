using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
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
using static System.Net.Mime.MediaTypeNames;

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
        bool isOnRepeat;
        bool isShuffle;
        ObservableCollection<Song> shuffledSongs;
        public MainWindow()
        {
            InitializeComponent();
            isPlaying = false;
            isOnRepeat = false;
            isShuffle = false;
            model = new ViewModel();
            this.DataContext = model;
            Button_Delete.IsEnabled = false;
            shuffledSongs = new ObservableCollection<Song>(model.GetSongs().OrderBy(x => Guid.NewGuid()));
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
            //count = 0;
            int count;
            var songsType = model.GetSongs();
            try
            {
                if (ListMusic.ItemsSource == shuffledSongs)
                {
                    songsType = shuffledSongs;
                }
                else if (ListMusic.ItemsSource == model.GetSongs())
                {
                    songsType = model.GetSongs();
                }
                count = 0;
                var imageplay = new System.Windows.Controls.Image { Source = new BitmapImage(new Uri("/Images/play.png", UriKind.Relative)) };
                if (isOnRepeat == true)
                {
                    mediaElement.Position = TimeSpan.Zero;
                    mediaElement.Play();

                    //mediaPlayer.Open(new Uri(@"Songs/" + model.GetName(ListMusic.SelectedItem as Song)));
                    //Task.Run(() =>
                    //{
                    //    mediaElement.Dispatcher.Invoke(() =>
                    //    {
                    //        //mediaElement.Source = new Uri(System.IO.Path.Combine(path, model.GetName(ListMusic.SelectedItem as Song) + ".mp3"));
                    //        mediaElement.Play();
                    //    });
                    //});
                }
                else
                {

                    if (songsType.Count > 1)
                    {
                        foreach (var song in songsType)
                        {
                            if ((ListMusic.SelectedItem as Song).Name == song.Name)
                            {
                                break;
                            }
                            count++;
                        }
                        if (count < songsType.Count - 1)
                        {
                            slider.Value = 0;
                            ListMusic.SelectedItem = songsType[count + 1];
                            //MessageBox.Show((count + 1).ToString());
                            if (!isPlaying)
                            {
                                mediaElement.Play();
                            }
                        }
                        else
                        {
                            count = 0;
                            Button_Play.Content = imageplay;
                            if (!isShuffle)
                            {
                                Button_Delete.IsEnabled = true;
                            }
                            isPlaying = false;
                            mediaElement.Stop();
                        }
                    }
                    else
                    {
                        Button_Play.Content = imageplay;
                        if (!isShuffle)
                        {
                            Button_Delete.IsEnabled = true;
                        }
                        isPlaying = false;
                        mediaElement.Stop();
                    }

                    //int currentIndex = ListMusic.SelectedIndex;

                    //// перевіряємо, чи вже програвся останній елемент
                    //if (currentIndex == ListMusic.Items.Count - 1)
                    //{
                    //    // якщо так, зупиняємо відтворення
                    //    mediaElement.Stop();
                    //    return;
                    //}

                    //// вибираємо наступну пісню зі списку
                    //ListMusic.SelectedIndex = currentIndex + 1;

                    //// отримуємо шлях до наступної пісні та відтворюємо її
                    ////string path = (ListMusic.SelectedItem as Song).Path;
                    ////mediaElement.Source = new Uri(path);
                    //mediaElement.Play();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //count = 0;
            //var imageplay = new Image { Source = new BitmapImage(new Uri("/Images/play.png", UriKind.Relative)) };
            //if (isOnRepeat)
            //{
            //    mediaElement.Position = TimeSpan.Zero;
            //    mediaElement.Play();
            //}
            //else
            //{
            //    var songs = model.GetSongs();
            //    int currentIndex = songs.IndexOf(ListMusic.SelectedItem as Song);
            //    if (currentIndex == songs.Count - 1) // якщо це була остання пісня
            //    {
            //        Button_Play.Content = imageplay;
            //        Button_Delete.IsEnabled = true;
            //        isPlaying = false;
            //        mediaElement.Stop();
            //    }
            //    else // якщо є наступна пісня
            //    {
            //        ListMusic.SelectedItem = songs[currentIndex + 1];
            //        mediaElement.Play();
            //    }
            //}
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
                if (isPlaying)
                {
                    Storyboard storyboard = (Storyboard)FindResource("ZoomAnimation");
                    storyboard.Begin();
                }
                else
                {
                    Storyboard storyboard = (Storyboard)FindResource("ZoomAnimation");
                    storyboard.Stop();
                }
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
            //ListMusic.ItemsSource = model.Songs;
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
                if (ListMusic.SelectedItem != null && !isPlaying && !isShuffle)
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
                    var imageplay = new System.Windows.Controls.Image { Source = new BitmapImage(new Uri("/Images/play.png", UriKind.Relative)) };
                    var imagepause = new System.Windows.Controls.Image { Source = new BitmapImage(new Uri("/Images/pause.png", UriKind.Relative)) };
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
                        if (!isShuffle)
                        {
                            Button_Delete.IsEnabled = true;
                        }
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

        private void Button_Repeat_Click(object sender, RoutedEventArgs e)
        {
            if (isOnRepeat == false)
            {
                Button_Repeat.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2480b5"));
                isOnRepeat = true;
            }
            else
            {
                Button_Repeat.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34a8eb"));
                isOnRepeat = false;
            }
        }

        private void Button_Shuffle_Click(object sender, RoutedEventArgs e)
        {
            if (isShuffle == false)
            {
                Button_Shuffle.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2480b5"));
                isShuffle = true;
                Button_Add.IsEnabled = false;
                Button_Delete.IsEnabled = false;
                shuffledSongs = new ObservableCollection<Song>(model.GetSongs().OrderBy(x => Guid.NewGuid()));
                ListMusic.ItemsSource = shuffledSongs;
            }
            else
            {
                Button_Shuffle.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34a8eb"));
                isShuffle = false;
                Button_Add.IsEnabled = true;
                Button_Delete.IsEnabled = true;
                ListMusic.ItemsSource = model.Songs;
            }
        }

        private void Button_Next_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;
            var selectedsong = ListMusic.SelectedItem as Song;
            //var imagepause = new Image { Source = new BitmapImage(new Uri("/Images/pause.png", UriKind.Relative)) };
            var songsType = model.GetSongs();
                if (ListMusic.ItemsSource == shuffledSongs)
                {
                    songsType = shuffledSongs;
                }
                else if (ListMusic.ItemsSource == model.GetSongs())
                {
                    songsType = model.GetSongs();
                }
                if (selectedsong != null)
                {
                if (songsType.Count > 1)
                {
                    foreach (var song in songsType)
                    {
                        if (selectedsong.Name == song.Name)
                        {
                            break;
                        }
                        count++;
                    }
                    if (count == songsType.Count - 1)
                    {
                        count = 0;
                        slider.Value = 0;
                        ListMusic.SelectedItem = songsType[count];
                        //MessageBox.Show((count + 1).ToString());
                        if (isPlaying)
                        {
                            mediaElement.Play();
                        }
                    }
                    else
                    {
                        slider.Value = 0;
                        ListMusic.SelectedItem = songsType[count + 1];
                        //MessageBox.Show((count + 1).ToString());
                        if (isPlaying)
                        {
                            mediaElement.Play();
                        }
                    }
                    //if (count < model.GetSongs().Count - 1)
                    //{
                    //    slider.Value = 0;
                    //    ListMusic.SelectedItem = model.GetSongs()[count + 1];
                    //    //MessageBox.Show((count + 1).ToString());
                    //    if (!isPlaying)
                    //    {
                    //        mediaElement.Play();
                    //    }
                    //}
                    //else
                    //{
                    //    //count = 0;
                    //    //Button_Play.Content = imageplay;
                    //    //Button_Delete.IsEnabled = true;
                    //    //isPlaying = false;
                    //    //mediaElement.Stop();
                    //}
                }
                else
                {
                    //Button_Play.Content = imageplay;
                    //Button_Delete.IsEnabled = false;
                    //isPlaying = true;
                    //mediaElement.Stop();
                    string path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Songs");
                    mediaElement.Source = new Uri(System.IO.Path.Combine(path, model.GetName(selectedsong) + ".mp3"));
                    if (isPlaying)
                    {
                        mediaElement.Play();
                    }
                    //Button_Play.Content = imagepause;
                    model.SelectedName = model.GetName(selectedsong);
                }
            }

        }

        private void Button_Previous_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;
            var selectedsong = ListMusic.SelectedItem as Song;
            //var imagepause = new Image { Source = new BitmapImage(new Uri("/Images/pause.png", UriKind.Relative)) };
            var songsType = model.GetSongs();
            if (ListMusic.ItemsSource == shuffledSongs)
            {
                songsType = shuffledSongs;
            }
            else if (ListMusic.ItemsSource == model.GetSongs())
            {
                songsType = model.GetSongs();
            }
            if (selectedsong != null)
            {
                if (songsType.Count > 1)
                {
                    foreach (var song in songsType)
                    {
                        if (selectedsong.Name == song.Name)
                        {
                            break;
                        }
                        count++;
                    }
                    if (count == 0)
                    {
                        count = songsType.Count - 1;
                        slider.Value = 0;
                        ListMusic.SelectedItem = songsType[count];
                        //MessageBox.Show((count + 1).ToString());
                        if (isPlaying)
                        {
                            mediaElement.Play();
                        }
                    }
                    else
                    {
                        slider.Value = 0;
                        ListMusic.SelectedItem = songsType[count - 1];
                        //MessageBox.Show((count + 1).ToString());
                        if (isPlaying)
                        {
                            mediaElement.Play();
                        }
                    }
                    //if (count < model.GetSongs().Count - 1)
                    //{
                    //    slider.Value = 0;
                    //    ListMusic.SelectedItem = model.GetSongs()[count + 1];
                    //    //MessageBox.Show((count + 1).ToString());
                    //    if (!isPlaying)
                    //    {
                    //        mediaElement.Play();
                    //    }
                    //}
                    //else
                    //{
                    //    //count = 0;
                    //    //Button_Play.Content = imageplay;
                    //    //Button_Delete.IsEnabled = true;
                    //    //isPlaying = false;
                    //    //mediaElement.Stop();
                    //}
                }
                else
                {
                    //Button_Play.Content = imageplay;
                    //Button_Delete.IsEnabled = false;
                    //isPlaying = true;
                    //mediaElement.Stop();
                    string path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Songs");
                    mediaElement.Source = new Uri(System.IO.Path.Combine(path, model.GetName(selectedsong) + ".mp3"));
                    if (isPlaying)
                    {
                        mediaElement.Play();
                    }
                    //Button_Play.Content = imagepause;
                    model.SelectedName = model.GetName(selectedsong);
                }
            }

        }
    }
    
}
