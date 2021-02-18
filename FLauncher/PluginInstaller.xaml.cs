using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.Json;
using System.Text.Json.Serialization;
using Octokit;
using System.IO;
using System.Net;
using AdonisUI.Controls;
using MessageBox = AdonisUI.Controls.MessageBox;

namespace FLauncher
{
    /// <summary>
    /// Interaction logic for PluginInstaller.xaml
    /// </summary>
    public partial class PluginInstaller : AdonisUI.Controls.AdonisWindow
    {
        Credentials creds;
        public PluginInstaller()
        {
            InitializeComponent();
        }

        private void AdonisWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try { creds = new Credentials(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/FLauncher" + "/GithubKey.txt")); } catch { }

            LoadPlugins();
        }

        private void LoadPlugins()
        {
            var github = new GitHubClient(new ProductHeaderValue("FLauncher"));
            github.Credentials = creds;

            var contents = github.Repository.Content.GetAllContents("Oliveoil1", "Flauncher.Plugins", "metadata").Result;

            foreach(RepositoryContent r in contents)
            {
                try
                {
                    var pInfo = JsonSerializer.Deserialize<PluginInfo>(new WebClient().DownloadString(r.DownloadUrl));

                    var pInfoListItem = new ListBoxItem();
                    pInfoListItem.Content = pInfo.Name;
                    pInfoListItem.ToolTip = pInfo.Description + " | Version " + pInfo.Version;
                    pInfoListItem.MouseDoubleClick += PInfoListItem_MouseDoubleClick;
                    pInfoListItem.Tag = pInfo.DownloadUrl;

                    PluginInfoList.Items.Add(pInfoListItem);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
            }
        }

        private void PInfoListItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string toDownload = (sender as ListBoxItem).Tag.ToString();

            MessageBoxModel downloadMsg = new MessageBoxModel
            {
                Text = "Downloading file from this url: " + toDownload,
                Caption = "Confirm download",
                Icon = AdonisUI.Controls.MessageBoxImage.Information,
                Buttons = new[]
                {
                    MessageBoxButtons.Ok(),
                    MessageBoxButtons.Cancel(),
                },

            };

            MessageBox.Show(downloadMsg);

            if (downloadMsg.Result == AdonisUI.Controls.MessageBoxResult.OK)
            {
                new WebClient().DownloadFile(new Uri(toDownload), Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\FLauncher\Plugins\test");
                MessageBox.Show("Plugin download complete, restart FLauncher to use it", "Download Complete");
            }
        }

        private void CreatePluginMetadata_Click(object sender, RoutedEventArgs e)
        {
            new PluginMetadataCreate().Show();
        }
    }

    public class PluginInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string DownloadUrl { get; set; }
        public string Version { get; set; }
    }
}
