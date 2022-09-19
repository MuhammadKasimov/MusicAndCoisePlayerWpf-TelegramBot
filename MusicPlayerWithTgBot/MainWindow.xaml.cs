using MusicPlayerWithTgBot.Controllers;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

#pragma warning disable
namespace MusicPlayerWithTgBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int count;
        private DispatcherTimer Alarm;
        private readonly DispatcherTimer _timer;
        private bool isPlaying;
        private DateTime _currentTime;
        private bool isDraging = false;
        private TelegramBotClient client;


        public MainWindow()
        {
            InitializeComponent();
            client = new TelegramBotClient("5210888930:AAGOcXbc6xLIDh_i-lrHN0h9kdfl-26mUjk");
            if (!Directory.Exists("Voises"))
                Directory.CreateDirectory("Voises");
            if (!Directory.Exists("Musics"))
                Directory.CreateDirectory("Musics");

            MusicMediaEl.Volume = (double)VolumeSlider.Value;
            LoadMusic();
            PLayOrPauseBtn.IsEnabled = false;
            isPlaying = false;
            Alarm = new DispatcherTimer();
            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
            Alarm.Tick += Alarm_Tick;
            Alarm.Start();
            PlayOrStopIcon.Source = new BitmapImage(new Uri("Images/pause.png", UriKind.Relative));
            count = 0;
        }

        private void Alarm_Tick(object sender, EventArgs e)
        {
            _currentTime = DateTime.Now;

            foreach (var i in AudiosList.Items)
            {
                var alarmcntrl = (i as Button).Content as AlarmController;

                if (alarmcntrl.PlayTimeTxt.Text == _currentTime.ToString("HH:mm"))
                {
                    MusicMediaEl.Close();

                    alarmcntrl.PlayTimeTxt.Text = "";

                    var files = new DirectoryInfo("Musics").GetFiles();

                    var lastFile = files.FirstOrDefault(c => c.Name == alarmcntrl.NameOfMusicTxt.Text);

                    if (lastFile != null)
                    {
                        MusicMediaEl.Source = new Uri(lastFile.FullName, UriKind.RelativeOrAbsolute);
                        FileNameTxt.Text = lastFile.Name;
                        MusicMediaEl.Play();
                        if (!isPlaying)
                        {
                            PlayOrStopIcon.Source = new BitmapImage(new Uri("Images/play.png", UriKind.Relative));
                            count++;
                        }
                    }
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!isDraging)
                TimelineSlider.Value = MusicMediaEl.Position.TotalSeconds;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MusicMediaEl.Volume = (double)VolumeSlider.Value;
        }

        private void PLayOrPauseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (count % 2 == 0)
            {
                MusicMediaEl.Play();
                PlayOrStopIcon.Source = new BitmapImage(new Uri("Images/play.png", UriKind.Relative));
                isPlaying = true;
            }
            else
            {
                MusicMediaEl.Pause();
                PlayOrStopIcon.Source = new BitmapImage(new Uri("Images/pause.png", UriKind.Relative));
                isPlaying = false;
            }
            count++;
        }

        private void MusicMediaEl_MediaEnded(object sender, RoutedEventArgs e)
        {
            MusicMediaEl.Stop();
            PlayOrStopIcon.Source = new BitmapImage(new Uri("Images/pause.png", UriKind.Relative));
            count++;
        }

        private void MusicMediaEl_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (MusicMediaEl.NaturalDuration.HasTimeSpan)
                TimelineSlider.Maximum = MusicMediaEl.NaturalDuration.TimeSpan.Minutes * 60 + MusicMediaEl.NaturalDuration.TimeSpan.Seconds;
            _timer.Start();
            PLayOrPauseBtn.IsEnabled = true;
        }

        private void TimelineSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            isDraging = true;
        }

        private void TimelineSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            isDraging = false;
            MusicMediaEl.Position = new TimeSpan(0, 0, 0, (int)TimelineSlider.Value);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {

            client.StartReceiving();
            client.OnMessage += UpdateHandler;
        }

        private async void UpdateHandler(object sender, MessageEventArgs e)
        {
            if (e.Message.Type == MessageType.Audio)
            {

                var message = e.Message;

                var filePath = $"Musics/{message.Audio.FileName}";

                var tgFile = await client.GetFileAsync(message.Audio.FileId);


                if (System.IO.File.Exists(filePath))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    fileInfo.CreationTime = DateTime.Now;
                    fileInfo.CreationTimeUtc = DateTime.UtcNow;
                    this.Dispatcher.Invoke(() => LoadMusic());
                }
                else
                {
                    using (var file = new FileStream(filePath, FileMode.Create))
                    {
                        await client.DownloadFileAsync(tgFile.FilePath, file);

                        this.Dispatcher.Invoke(() => LoadMusic());
                    }
                }
            }

            else if (e.Message.Type == MessageType.Voice)
            {

                var message = e.Message;

                var filePath = $"Voises/{message.Voice.FileId}.mp3";

                var tgFile = await client.GetFileAsync(message.Voice.FileId);

                using (var file = new FileStream(filePath, FileMode.Create))
                {
                    await client.DownloadFileAsync(tgFile.FilePath, file);
                }

                this.Dispatcher.Invoke(() =>
                {
                    MusicMediaEl.Stop();
                });

                Process process = new Process();

                process.StartInfo.UseShellExecute = true;
                // You can start any process, HelloWorld is a do-nothing example.
                process.StartInfo.FileName = new DirectoryInfo("Voises").GetFiles()
                    .OrderBy(f => f.CreationTime)
                        .LastOrDefault().FullName;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

            }
        }

        private void LoadMusic()
        {
            AudiosList.Items.Clear();
            var files = new DirectoryInfo("Musics").GetFiles().OrderBy(f => f.CreationTime).ToList();
            files.ForEach(a =>
            {
                Button button = new Button()
                {
                    Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255)),
                    BorderThickness = new Thickness(0),
                    Padding = new Thickness(0),
                    Width = 780,
                    Height = 75
                };
                button.Click += PlayMusic;

                AlarmController alarmController = new AlarmController();
                alarmController.NameOfMusicTxt.Text = a.Name;

                button.Content = alarmController;
                AudiosList.Items.Add(button);
            });
        }

        private void PlayMusic(object sender, RoutedEventArgs e)
        {
            var cont = (sender as Button).Content as AlarmController;
            MusicMediaEl.Close();

            var files = new DirectoryInfo("Musics").GetFiles();

            var lastFile = files.FirstOrDefault(c => c.Name == cont.NameOfMusicTxt.Text);

            if (lastFile != null)
            {
                MusicMediaEl.Source = new Uri(lastFile.FullName, UriKind.RelativeOrAbsolute);
                FileNameTxt.Text = lastFile.Name;
                MusicMediaEl.Play();
                if (!isPlaying)
                {
                    PlayOrStopIcon.Source = new BitmapImage(new Uri("Images/play.png", UriKind.Relative));
                    count++;
                }
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            client.StopReceiving();
            Close();
        }
    }
}
