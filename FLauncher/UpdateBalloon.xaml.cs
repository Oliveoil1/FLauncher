using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FLauncher
{
    /// <summary>
    /// Interaction logic for UpdateBalloon.xaml
    /// </summary>
    public partial class UpdateBalloon : UserControl
    {
        Progress progress;
        public UpdateBalloon(bool updateAvaible, string v_tag = "null")
        {
            InitializeComponent();

            if(updateAvaible)
            {
                //Cancel.Visibility = Visibility.Visible;
                Update.Visibility = Visibility.Visible;
                Status.Content = "Update Available!\nUpdate to: " + v_tag;
            }
            else
            {
                //Cancel.Visibility = Visibility.Hidden;
                Update.Visibility = Visibility.Hidden;
                Status.Content = "No updates available";
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Update available, would you like to update?", "Update Available", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                try
                {
                    var client = new WebClient();
                    client.DownloadProgressChanged += Client_DownloadProgressChanged;
                    client.DownloadFileCompleted += Client_DownloadFileCompleted;

                    progress = new Progress("Downloading Update", "Updater");
                    progress.Show();

                    client.DownloadFileAsync(new Uri("https://github.com/OliveOil1/FLauncher/releases/latest/download/setup.exe"), Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\FLauncher" + @"\update.exe");
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            try
            {
                progress.Hide();
                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\update.exe", "/DIR=" + Directory.GetCurrentDirectory());
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progress.SetPercent(e.ProgressPercentage);
        }
    }
}
