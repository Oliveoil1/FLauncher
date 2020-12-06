﻿using System;
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

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
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
    }
}
