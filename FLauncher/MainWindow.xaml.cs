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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using Hardcodet.Wpf.TaskbarNotification;
using System.IO;
using CsvHelper;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Drawing;
using Octokit;
using Application = System.Windows.Application;
using ProductHeaderValue = Octokit.ProductHeaderValue;

namespace FLauncher
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		ShowMessageCommand s = new ShowMessageCommand();
		string[] appdataDirs = { Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/FLauncher", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/FLauncher/Plugins" };

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			CreateAppData();
			s.Execute("ShowWindow");
			Hide();
			//UpdateCheck();
		}

		private async void UpdateCheck()
        {
            s.Execute("ShowWindow");
			var github = new GitHubClient(new ProductHeaderValue("FLauncher"));

			var release = await github.Repository.Release.GetLatest("OliveOil1", "Flauncher");

			if (release.TagName != File.ReadAllText(Directory.GetCurrentDirectory() + @"\version.txt"))
            {
				MessageBoxResult result = MessageBox.Show("Update found, would you like to update?", "Update Found", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    new WebClient().DownloadFile("https://github.com/OliveOil1/FLauncher/releases/latest/download/setup.exe", "update.exe");

					Process.Start(Directory.GetCurrentDirectory() + @"\update.exe", "/CLOSEAPPLICATIONS /SILENT /DIR=" + Directory.GetCurrentDirectory());
                }
			}
			else
            {
				MessageBox.Show("No updates available");
            }
		}

		private void CreateAppData()
        {
			foreach(string dir in appdataDirs)
            {
				if (!Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}
			}

			if(!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/FLauncher/Aliases.csv"))
            {
				using (StreamWriter sw = File.CreateText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/FLauncher" + "/Aliases.csv"))
				{
					sw.WriteLine("alias,full_path");
					sw.WriteLine("g,https://www.google.com/search?q=");
				}
			}
		}

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
			Process.Start("explorer.exe", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\FLauncher");
		}

		private void ReloadPrefs(object sender, RoutedEventArgs e)
		{
			Process.Start(Process.GetCurrentProcess().MainModule.FileName);
			Application.Current.Shutdown();
		}

		private void ShowAbout(object sender, RoutedEventArgs e)
		{
			new About().ShowDialog();
		}

		private void CheckForUpdatesClick(object sender, RoutedEventArgs e)
        {
			UpdateCheck();
        }

        private void EditAliasClick(object sender, RoutedEventArgs e)
        {
			new AliasEditor().Show();
        }

        private void OptionsClick(object sender, RoutedEventArgs e)
        {
			new Options().Show();
        }

        private void TaskbarIcon1_Initialized(object sender, EventArgs e)
        {
#if DEBUG
			TaskbarIcon1.SetValue(IconProperty, new BitmapImage(new Uri("pack://application:,,,/FLauncher;component/Resources/FLauncherDebug.ico")));
#endif
		}
	}
}
