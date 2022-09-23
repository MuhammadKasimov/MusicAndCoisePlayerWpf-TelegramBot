using MusicPlayerWithTgBot.Constants;
using MusicPlayerWithTgBot.Controllers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
using Telegram.Bot.Types.ReplyMarkups;
using WindowsInput;
using WindowsInput.Native;

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
        IInputSimulator inputSimulator;
        private string token;
        ReplyKeyboardMarkup keyboardMarkup;
        public MainWindow()
        {
            InitializeComponent();
            inputSimulator = new InputSimulator();
            CreateAllDirectories();
            LoadLastMusic();
            token = JsonConvert.DeserializeObject<string>(File.ReadAllText(FilePathes.TOKEN));
            client = new TelegramBotClient(token);
            keyboardMarkup = new ReplyKeyboardMarkup();

            MusicMediaEl.Volume = (double)VolumeSlider.Value;
            LoadMusic();
            PLayOrPauseBtn.IsEnabled = false;
            isPlaying = false;
            Alarm = new DispatcherTimer();
            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
            Alarm.Tick += Alarm_Tick;

            KeyboardButton VoiseUpp = new KeyboardButton();
            KeyboardButton VoiseDown = new KeyboardButton();

            KeyboardButton Start = new KeyboardButton();
            KeyboardButton Pause = new KeyboardButton();

            KeyboardButton Stop = new KeyboardButton();

            KeyboardButton Mute = new KeyboardButton();
            KeyboardButton Clear = new KeyboardButton();

            VoiseUpp.Text = "Voise Upp";
            VoiseDown.Text = "Voise Down";

            Start.Text = "Play";
            Pause.Text = "Pause";
            Stop.Text = "Stop";

            Mute.Text = "Mute";
            Clear.Text = "Clear";

            KeyboardButton[] MuteClear = new KeyboardButton[] { Mute, Clear };

            KeyboardButton[] StartPause = { Start, Pause };


            KeyboardButton[] VoiseRegulators = new KeyboardButton[] { VoiseUpp, VoiseDown };

            var Buttons = new KeyboardButton[][] { VoiseRegulators, StartPause, new[] { Stop }, MuteClear };

            keyboardMarkup.Keyboard = Buttons;

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
            List<string> users = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(FilePathes.USERS));

            if (users.Any(u => e.Message.Chat.Id.ToString() == u))
            {
                if (e.Message.Type == MessageType.Text)
                {
                    if (e.Message.Text.ToLower() == "/start")
                    {
                        client.SendTextMessageAsync(e.Message.Chat.Id, "assalomu alaykum manga audio yoki voice jonating", replyMarkup: keyboardMarkup);
                    }

                    else if (e.Message.Text.ToLower() == "voise upp")
                    {
                        inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VOLUME_UP);
                        client.DeleteMessageAsync(e.Message.Chat.Id, e.Message.MessageId);
                    }
                    else if (e.Message.Text.ToLower() == "voise down")
                    {
                        inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VOLUME_DOWN);
                        client.DeleteMessageAsync(e.Message.Chat.Id, e.Message.MessageId);
                    }
                    else if (e.Message.Text.ToLower() == "play")
                    {
                        if (e.Message.ReplyToMessage != null && e.Message.ReplyToMessage.Type == MessageType.Audio)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                LoadChooosenMusic(e.Message.ReplyToMessage.Audio.FileName);
                                MusicMediaEl.Play();
                            });
                        }
                        else
                        {
                            this.Dispatcher.Invoke(() => MusicMediaEl.Play());
                        }
                        client.DeleteMessageAsync(e.Message.Chat.Id, e.Message.MessageId);
                    }
                    else if (e.Message.Text.ToLower() == "pause")
                    {
                        this.Dispatcher.Invoke(() => MusicMediaEl.Pause());
                        client.DeleteMessageAsync(e.Message.Chat.Id, e.Message.MessageId);
                    }
                    else if (e.Message.Text.ToLower() == "stop")
                    {
                        this.Dispatcher.Invoke(() => MusicMediaEl.Stop());
                        client.DeleteMessageAsync(e.Message.Chat.Id, e.Message.MessageId);
                    }
                    else if (e.Message.Text.ToLower() == "clear")
                    {
                        this.Dispatcher.Invoke(() => ClearAllFiles());
                        client.DeleteMessageAsync(e.Message.Chat.Id, e.Message.MessageId);
                    }
                    else if (e.Message.Text.ToLower() == "mute")
                    {
                        inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VOLUME_MUTE);
                        client.DeleteMessageAsync(e.Message.Chat.Id, e.Message.MessageId);
                    }
                    else if (e.Message.Text.ToLower().StartsWith("alarm=") && e.Message.ReplyToMessage != null && e.Message.ReplyToMessage.Type == MessageType.Audio)
                    {
                        foreach (var i in AudiosList.Items)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                var alarmcntrl = (i as Button).Content as AlarmController;

                                if (alarmcntrl.NameOfMusicTxt.Text == e.Message.ReplyToMessage.Audio.FileName)
                                {
                                    alarmcntrl.PlayTimeTxt.Text = e.Message.Text.ToLower().Replace("alarm=", "");
                                }
                            });

                        }
                    }
                }
                if (e.Message.Type == MessageType.Audio)
                {

                    var message = e.Message;

                    var filePath = $"Musics/{message.Audio.FileName}";
                    Telegram.Bot.Types.File tgFile;
                    try
                    {
                        tgFile = await client.GetFileAsync(message.Audio.FileId);
                    }
                    catch
                    {
                        client.SendTextMessageAsync(message.Chat.Id, "file 20 mb dan oshmasligi shart");
                        return;
                    }

                    if (System.IO.File.Exists(filePath))
                    {
                        FileInfo fileInfo = new FileInfo(filePath);
                        fileInfo.CreationTime = DateTime.Now;
                        fileInfo.CreationTimeUtc = DateTime.UtcNow;
                        this.Dispatcher.Invoke(() => LoadMusic());
                    }
                    else
                    {
                        var file = new FileStream(filePath, FileMode.Create);
                        await client.DownloadFileAsync(tgFile.FilePath, file);
                        this.Dispatcher.Invoke(() => { LoadMusic(); });
                    }
                    this.Dispatcher.Invoke(() => MusicMediaEl.Source = new Uri(filePath, UriKind.Relative));

                    client.SendTextMessageAsync(message.Chat.Id, "file yuklandi vaqt qoyish uchun ashulani reply qqilgan holda 'alarm=hh:mm' yozing\n uni hoziroq yoqish uchun startni bosing");
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
            else
            {
                client.SendTextMessageAsync(e.Message.Chat.Id, "Uzr sizni tanimiman");
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
        private void LoadLastMusic()
        {
            var files = new DirectoryInfo("Musics").GetFiles();

            var lastFile = files.OrderBy(f => f.CreationTime).LastOrDefault();

            if (lastFile != null)
            {
                MusicMediaEl.Source = new Uri(lastFile.FullName, UriKind.RelativeOrAbsolute);
            }
        }

        private void LoadChooosenMusic(string name)
        {
            var files = new DirectoryInfo("Musics").GetFiles();

            var lastFile = files.FirstOrDefault(f => f.Name == name);

            if (lastFile != null)
            {
                MusicMediaEl.Source = new Uri(lastFile.FullName, UriKind.RelativeOrAbsolute);
            }

        }


        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            client.StopReceiving();
            Close();
        }

        private void CreateAllDirectories()
        {
            if (!Directory.Exists(FilePathes.VOISE_MESSAGES))
                Directory.CreateDirectory(FilePathes.VOISE_MESSAGES);
            if (!Directory.Exists(FilePathes.AUDIOS))
                Directory.CreateDirectory(FilePathes.AUDIOS);
            if (!Directory.Exists("Configurations"))
            {
                Directory.CreateDirectory("Configurations");
                File.WriteAllText(FilePathes.TOKEN, JsonConvert.SerializeObject("5210888930:AAGOcXbc6xLIDh_i-lrHN0h9kdfl-26mUjk"));
            }
            if (!File.Exists(FilePathes.TOKEN))
                File.WriteAllText(FilePathes.TOKEN, JsonConvert.SerializeObject("5210888930:AAGOcXbc6xLIDh_i-lrHN0h9kdfl-26mUjk"));

            if (String.IsNullOrEmpty(File.ReadAllText(FilePathes.TOKEN)))
                File.WriteAllText(FilePathes.TOKEN, JsonConvert.SerializeObject("5210888930:AAGOcXbc6xLIDh_i-lrHN0h9kdfl-26mUjk"));

            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
                File.WriteAllText(FilePathes.USERS, "[]");
            }
            if (!File.Exists(FilePathes.USERS))
                File.Create(FilePathes.USERS);
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().ShowDialog();
        }

        private void ClearAllFiles()
        {
            var files = new DirectoryInfo("Musics").GetFiles().Select(f => f.FullName).ToList();
            for (int i = 0; i < files.Count; i++)
            {
                File.Delete(files[i]);
            }
            files = new DirectoryInfo("Voises").GetFiles().Select(f => f.FullName).ToList();
            for (int i = 0; i < files.Count; i++)
            {
                File.Delete(files[i]);
            }
            LoadMusic();
        }
        private void DeleteChoosenMusic(string fileName)
        {
            var files = new DirectoryInfo("Musics").GetFiles().FirstOrDefault(f => f.Name == fileName);

            File.Delete("Musics/" + fileName);
            LoadMusic();
        }
        private void DeleteChoosenVoise(string fileName)
        {
            var files = new DirectoryInfo("Voises").GetFiles().FirstOrDefault(f => f.Name == fileName);

            File.Delete("Voises/" + fileName);
        }

        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch { }
        }
    }
}
