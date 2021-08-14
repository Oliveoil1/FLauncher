using AdonisUI;
using Octokit;
using System;
using System.Diagnostics;
using System.IO;
using System.Timers;
using System.Windows;
using Application = System.Windows.Application;
using MessageBox = AdonisUI.Controls.MessageBox;
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
		Timer checkForTime = new Timer(1800000);
		Credentials creds;

		public MainWindow()
		{
			InitializeComponent();
			checkForTime.Elapsed += new ElapsedEventHandler(checkForTime_Elapsed);
			checkForTime.Enabled = true;

			ResourceLocator.SetColorScheme(Application.Current.Resources, Settings1.Default.IsDark ? ResourceLocator.LightColorScheme : ResourceLocator.DarkColorScheme);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			CreateAppData();
			try { creds = new Credentials(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/FLauncher" + "/GithubKey.txt")); } catch { }
			s.Execute("ShowWindow");
			Hide();
			if (Settings1.Default.AutoUpdate)
				UpdateCheck(true);
		}

		private async void UpdateCheck(bool auto)
		{
			//s.Execute("ShowWindow");
			var github = new GitHubClient(new ProductHeaderValue("FLauncher"));
			github.Credentials = creds;

			Release release = new Release();

			try
			{
				release = await github.Repository.Release.GetLatest("OliveOil1", "Flauncher");
			}
			catch (Exception ex)
			{
				if (!auto)
					MessageBox.Show(ex.Message, "Error");
				return;
			}


			if (release.TagName != File.ReadAllText(Directory.GetCurrentDirectory() + @"\version.txt"))
			{
				if (auto)
					NotifyIcon1.ShowCustomBalloon(new UpdateBalloon(true, release.TagName), System.Windows.Controls.Primitives.PopupAnimation.Fade, 10000);
				else
					NotifyIcon1.ShowCustomBalloon(new UpdateBalloon(true, release.TagName), System.Windows.Controls.Primitives.PopupAnimation.Fade, 10000);
			}
			else
			{
				if (!auto)
					NotifyIcon1.ShowCustomBalloon(new UpdateBalloon(false), System.Windows.Controls.Primitives.PopupAnimation.Fade, 10000);
			}
		}

		private void CreateAppData()
		{
			foreach (string dir in appdataDirs)
			{
				if (!Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}
			}

			if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/FLauncher/Aliases.csv"))
			{
				using (StreamWriter sw = File.CreateText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/FLauncher" + "/Aliases.csv"))
				{
					sw.WriteLine("alias,full_path,parameters");
					sw.WriteLine("g,https://www.google.com/search?q=,");
				}
			}
			if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/FLauncher/GithubKey.txt"))
			{
				using (StreamWriter sw = File.CreateText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/FLauncher" + "/GithubKey.txt"))
				{
					sw.WriteLine("[API KEY HERE]");
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
			UpdateCheck(false);
		}

		private void OptionsClick(object sender, RoutedEventArgs e)
		{
			new Options().Show();
		}

		private void checkForTime_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (Settings1.Default.AutoUpdate)
				this.Dispatcher.Invoke(() => UpdateCheck(true));
		}


		private void TaskbarIcon1_Initialized(object sender, EventArgs e)
		{

		}
	}
}
