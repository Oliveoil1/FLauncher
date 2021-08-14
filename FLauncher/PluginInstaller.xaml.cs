using AdonisUI.Controls;
using Octokit;
using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

			foreach (RepositoryContent r in contents)
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
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error");
				}
			}
		}

		private void PInfoListItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			ListBoxItem senderItem = sender as ListBoxItem;
			string toDownload = senderItem.Tag.ToString();

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
				new WebClient().DownloadFile(new Uri(toDownload), Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\FLauncher\Plugins\" + senderItem.Content + ".dll");
				MessageBox.Show("Plugin download complete, restart FLauncher to use it", "Download Complete");
			}
		}

		private void CreatePluginMetadata_Click(object sender, RoutedEventArgs e)
		{
			new PluginMetadataCreate().ShowDialog();
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
