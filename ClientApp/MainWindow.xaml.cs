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
        DispatcherTimer timer;
        bool isPlaying;
        bool isOnRepeat;
        bool isShuffle;
        bool isVolumeVisible;
        ObservableCollection<Song> shuffledSongs;
        ObservableCollection<Song> searchSongs;
        string newFolderPath;
        public MainWindow()
        {
            InitializeComponent();
            VolumeSlider.Visibility = Visibility.Hidden;
            Delete.Visibility = Visibility.Visible;
            mediaElement.Volume = (double)10/100;
            isPlaying = false;
            isOnRepeat = false;
            isShuffle = false;
            isVolumeVisible = false;
            model = new ViewModel();
            this.DataContext = model;
            Button_Delete.IsEnabled = false;
            shuffledSongs = new ObservableCollection<Song>(model.GetSongs().OrderBy(x => Guid.NewGuid()));
            searchSongs = new ObservableCollection<Song>();
            newFolderPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Songs");
            if (!Directory.Exists(newFolderPath))
            {
                Directory.CreateDirectory(newFolderPath);
            }
            if (Directory.GetFiles(newFolderPath).Length != 0)
            {
                string[] files = Directory.GetFiles(newFolderPath, "*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
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
                }
            }
            catch (Exception ex)
            {
                
            }
        }
        private void MediaElement_MediaOpened(object? sender, EventArgs e)
        {
            try
            {
                model.SongDuration = string.Format("{0:mm\\:ss}/{1:mm\\:ss}", mediaElement.Position, mediaElement.NaturalDuration.TimeSpan);
                timer.Start();
                slider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            }
            catch (Exception ex) 
            { 
                
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
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
                
            }

        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
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
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    ListMusic.ItemsSource = model.Songs;
                    searchTextBox.Text = "";
                    File.Copy(openFileDialog.FileName, System.IO.Path.Combine(newFolderPath, openFileDialog.SafeFileName));
                    model.AddSongs(new Song(System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName)));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                
            }
        }

        private void ListMusic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedsong = ListMusic.SelectedItem as Song;
                if (ListMusic.SelectedItem != null)
                {
                    string path = newFolderPath;
                    mediaElement.Source = new Uri(System.IO.Path.Combine(path, model.GetName(selectedsong) + ".mp3"));
                }
                if (ListMusic.SelectedItem != null && !isPlaying && !isShuffle)
                {   
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
                        File.Delete(System.IO.Path.Combine(newFolderPath, model.GetName(selectedsong) + ".mp3"));
                        model.ClearSong(selectedsong);
                    }
                    catch (Exception ex)
                    {
                        
                    }
        }

        private void Button_Play_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ListMusic.SelectedItem != null)
                {
                    Delete.Visibility = Visibility.Hidden;
                    model.SelectedName = model.GetName(ListMusic.SelectedItem as Song);
                    var imageplay = new System.Windows.Controls.Image { Source = new BitmapImage(new Uri("/Images/play.png", UriKind.Relative)) };
                    var imagepause = new System.Windows.Controls.Image { Source = new BitmapImage(new Uri("/Images/pause.png", UriKind.Relative)) };
                    if (isPlaying == false)
                    {
                        Button_Play.Content = imagepause;
                        Button_Delete.IsEnabled = false;
                        isPlaying = true;
                        mediaElement.Play();
                    }
                    else
                    {
                        Delete.Visibility = Visibility.Visible;
                        Button_Play.Content = imageplay;
                        isPlaying = false;
                        if (!isShuffle)
                        {
                            Button_Delete.IsEnabled = true;
                        }
                        mediaElement.Pause();
                    }
                }
            }
            catch (Exception ex)
            {
                
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
                searchTextBox.Text = "";
                Button_Shuffle.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2480b5"));
                isShuffle = true;
                Button_Add.IsEnabled = false;
                Button_Delete.IsEnabled = false;
                shuffledSongs = new ObservableCollection<Song>(model.GetSongs().OrderBy(x => Guid.NewGuid()));
                ListMusic.ItemsSource = shuffledSongs;
            }
            else
            {
                searchTextBox.Text = "";
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
            var songsType = model.GetSongs();
                if (ListMusic.ItemsSource == shuffledSongs)
                {
                    songsType = shuffledSongs;
                }
                else if (ListMusic.ItemsSource == model.GetSongs())
                {
                    songsType = model.GetSongs();
                }
                else if (ListMusic.ItemsSource == searchSongs)
                {
                    songsType= searchSongs;
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
                        if (isPlaying)
                        {
                            mediaElement.Play();
                        }
                    }
                    else
                    {
                        slider.Value = 0;
                        ListMusic.SelectedItem = songsType[count + 1];
                        if (isPlaying)
                        {
                            mediaElement.Play();
                        }
                    }
                }
                else
                {
                    string path = newFolderPath;
                    mediaElement.Source = new Uri(System.IO.Path.Combine(path, model.GetName(selectedsong) + ".mp3"));
                    if (isPlaying)
                    {
                        mediaElement.Play();
                    }
                    model.SelectedName = model.GetName(selectedsong);
                }
            }

        }

        private void Button_Previous_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;
            var selectedsong = ListMusic.SelectedItem as Song;
            var songsType = model.GetSongs();
            if (ListMusic.ItemsSource == shuffledSongs)
            {
                songsType = shuffledSongs;
            }
            else if (ListMusic.ItemsSource == model.GetSongs())
            {
                songsType = model.GetSongs();
            }
            else if (ListMusic.ItemsSource == searchSongs)
            {
                songsType = searchSongs;
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
                        if (isPlaying)
                        {
                            mediaElement.Play();
                        }
                    }
                    else
                    {
                        slider.Value = 0;
                        ListMusic.SelectedItem = songsType[count - 1];
                        if (isPlaying)
                        {
                            mediaElement.Play();
                        }
                    }
                }
                else
                {
                    string path = newFolderPath;
                    mediaElement.Source = new Uri(System.IO.Path.Combine(path, model.GetName(selectedsong) + ".mp3"));
                    if (isPlaying)
                    {
                        mediaElement.Play();
                    }
                    model.SelectedName = model.GetName(selectedsong);
                }
            }

        }
        private void Search(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {  
                ListMusic.ItemsSource = model.Songs;
                model.SongSize = $"Songs Count: {model.GetSongs().Count}";
            }
            else
            {
                searchSongs = new ObservableCollection<Song>();
                Song song = new Song(searchText);
                foreach (var item in model.GetSongs())
                {
                    if(item.Name.ToLower().Contains(song.Name))
                    {
                        searchSongs.Add(item);
                    }
                }
                ListMusic.ItemsSource = searchSongs;
                model.SongSize = $"Songs Count: {ListMusic.Items.Count}";
            }
        }
        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = ((TextBox)sender).Text;
            searchText = searchText.ToLower();
            Search(searchText);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaElement.Volume = (double)VolumeSlider.Value / 100;
        }
        private void Volume_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(isVolumeVisible == false)
            {
                VolumeSlider.Visibility = Visibility.Visible;
                isVolumeVisible = true;
            }
            else
            {
                VolumeSlider.Visibility = Visibility.Hidden;
                isVolumeVisible = false;
            }    
        }
        private void Delete_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ListMusic.ItemsSource = model.Songs;
            foreach (var song in model.GetSongs())
            {
                File.Delete(System.IO.Path.Combine(newFolderPath, model.GetName(song) + ".mp3"));
            }
            model.ClearAll();
            searchTextBox.Text = "";
        }

        private void VolumeSlider_MouseEnter(object sender, MouseEventArgs e)
        {
            model.VolumeOfMusic = $"Volume: {(int)VolumeSlider.Value}%";
        }
    }
}