using AdonisUI;
using AdonisUI.Controls;
using CsvHelper;
using Microsoft.Win32;
using PluginBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MessageBox = AdonisUI.Controls.MessageBox;

namespace FLauncher
{
	/// <summary>
	/// Interaction logic for Options.xaml
	/// </summary>
	public partial class Options : AdonisUI.Controls.AdonisWindow
	{
		bool override_dialog = false;
		List<Alias> aliases;

		enum msgBoxIds
		{
			DONT_PROMPT
		}

		public Options()
		{
			InitializeComponent();
		}

		public static List<String> pluginNames = new List<string>();

		MessageBoxModel applyChanges = new MessageBoxModel
		{
			Text = "You may need to restart to apply all changes",
			Caption = "Apply changes",
			Icon = AdonisUI.Controls.MessageBoxImage.Information,
			Buttons = new[]
			{
				MessageBoxButtons.Ok(),
				MessageBoxButtons.Custom("Don't show again", 69 as object)
			},

		};

		MessageBoxModel installedPlugin = new MessageBoxModel
		{
			Text = "You need to restart before being able to use this plugin",
			Caption = "Installation Successfull",
			Icon = AdonisUI.Controls.MessageBoxImage.Information,
			Buttons = new[]
			{
				MessageBoxButtons.Ok()
			},

		};

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/FLauncher" + "/Aliases.csv"))
				{
					Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/FLauncher");
					using (StreamWriter sw = File.CreateText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/FLauncher" + "/Aliases.csv"))
					{
						sw.WriteLine("alias,full_path,");
						sw.WriteLine("g,https://www.google.com/search?q=,");
					}
				}
				var reader = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/FLauncher" + "/Aliases.csv");
				var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
				var records = csv.GetRecords<Alias>();

				aliases = records.ToList();


			}
			catch (Exception ex)
			{
				AdonisUI.Controls.MessageBox.Show(ex.Message);
			}
			AliasGrid.ItemsSource = aliases;

			foreach (IPlugin plugin in Runbox.plugins)
			{
				ListBoxItem listBoxItem = new ListBoxItem();
				listBoxItem.Content = plugin.name;
				listBoxItem.ToolTip = plugin.description;
				listBoxItem.Tag = plugin;
				PluginList.Items.Add(listBoxItem);
			}

			Autoupdate.IsChecked = Settings1.Default.AutoUpdate;
			IsDark.IsChecked = !Settings1.Default.IsDark;
			AutoReload.IsChecked = Settings1.Default.AutoReload;
			MarginUpDown.NUDTextBox.Text = Settings1.Default.DisplayMargin.ToString();

			RadioButton radioButtonSelect = (RadioButton)this.FindName(Settings1.Default.DisplayMode);
			radioButtonSelect.IsChecked = true;
		}

		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void Autoupdate_Click(object sender, RoutedEventArgs e)
		{
			Settings1.Default.AutoUpdate = (bool)Autoupdate.IsChecked;
			Settings1.Default.Save();
		}

		private void Open_Data_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start("explorer.exe", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\FLauncher");
		}

		private void Reload_App_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
			override_dialog = true;
			Application.Current.Shutdown();
		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			String to_save = "alias,full_path,parameters\n";

			foreach (Alias alias in AliasGrid.ItemsSource.Cast<Alias>())
			{
				to_save += alias.alias + "," + alias.full_path + "," + alias.parameters + "\n";
			}
			try
			{
				File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/FLauncher" + "/Aliases.csv", to_save);
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Save Failed", AdonisUI.Controls.MessageBoxButton.OK);
			}
			Settings1.Default.Save();

			if (Settings1.Default.PromptApplyChanges == true)
			{
				MessageBox.Show(applyChanges);

				if (applyChanges.Result == AdonisUI.Controls.MessageBoxResult.Custom)
				{
					Settings1.Default.PromptApplyChanges = false;
					Settings1.Default.Save();
				}

			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Save_Click(this, new RoutedEventArgs());
		}

		private void IsDark_Click(object sender, RoutedEventArgs e)
		{
			Settings1.Default.IsDark = !(bool)IsDark.IsChecked;
			ResourceLocator.SetColorScheme(Application.Current.Resources, Settings1.Default.IsDark ? ResourceLocator.LightColorScheme : ResourceLocator.DarkColorScheme);
		}

		private void AutoReload_Click(object sender, RoutedEventArgs e)
		{
			Settings1.Default.AutoReload = (bool)AutoReload.IsChecked;
		}

		private void DisplayModeRadio_Click(object sender, RoutedEventArgs e)
		{
			Settings1.Default.DisplayMode = (sender as RadioButton).Name;
		}

		private void NumericUpDown_ValueChanged(object sender, RoutedEventArgs e)
		{
			int margin;
			int.TryParse(MarginUpDown.NUDTextBox.Text, out margin);
			Settings1.Default.DisplayMargin = margin;
		}

		private void Download_Plugin_Click(object sender, RoutedEventArgs e)
		{
			new PluginInstaller().ShowDialog();
		}

		private void Install_Plugin_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();

			openFileDialog.Filter = "Dynamic-Link Libraries (*.dll)|*.dll|All Files (*.*)|*.*";

			Nullable<bool> result = openFileDialog.ShowDialog();

			if (result == true)
			{
				File.Copy(openFileDialog.FileName, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/FLauncher/Plugins/" + openFileDialog.SafeFileName);
				MessageBox.Show(installedPlugin);
			}
		}

		private void PluginList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void Create_Alias_Click(object sender, RoutedEventArgs e)
		{
			var aliasPopup = new CreateAliasPopup();
			aliasPopup.ShowDialog();

			if (aliasPopup.ok == true)
			{
				aliases.Add(aliasPopup.newAlias);

				AliasGrid.Items.Refresh();
			}
		}
	}
}
