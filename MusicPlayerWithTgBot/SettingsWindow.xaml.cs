using MusicPlayerWithTgBot.Constants;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace MusicPlayerWithTgBot
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void TokenBtnSave_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(FilePathes.TOKEN, JsonConvert.SerializeObject(TokenTxt.Text));
        }

        private void UserBtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(File.ReadAllText(FilePathes.USERS)))
                File.WriteAllText(FilePathes.USERS, "[]");
            var info = JsonConvert.DeserializeObject<IEnumerable<string>>(File.ReadAllText(FilePathes.USERS));
            info = info.Append(UserTxt.Text);
            File.WriteAllText(FilePathes.USERS, JsonConvert.SerializeObject(info));
        }

        private void UserDeleteBtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(File.ReadAllText(FilePathes.USERS)))
                File.WriteAllText(FilePathes.USERS, "[]");
            var info = JsonConvert.DeserializeObject<IEnumerable<string>>(File.ReadAllText(FilePathes.USERS));
            info = info.Where(i => i != UserDeleteTxt.Text);
            File.WriteAllText(FilePathes.USERS, JsonConvert.SerializeObject(info));
        }
    }
}
