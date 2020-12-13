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
using System.Globalization;
using CsvHelper;
using Path = System.IO.Path;
using PluginBase;
using System.Reflection;
using System.Runtime.Loader;

namespace FLauncher
{
	/// <summary>
	/// Interaction logic for Runbox.xaml
	/// </summary>
	public partial class Runbox : Window
	{
		public static List<Alias> aliases;
		public static List<IPlugin> plugins = new List<IPlugin>();

		public Runbox()
		{
			InitializeComponent();
		}

		String textEntered = "";

		private void Input_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				textEntered = Input.Text;

				bool inputHandled = false;

				foreach(IPlugin plugin in plugins)
                {
					inputHandled = plugin.CommandEntered(textEntered);
                }

				if(inputHandled == true)
                {
					return;
                }

				Process process = new Process();
				ProcessStartInfo processStartInfo = new ProcessStartInfo();

				foreach (Alias _alias in aliases)
				{
					//MessageBox.Show(_alias.alias + " " +_alias.full_path);
					if (textEntered == _alias.alias)
					{
						textEntered = _alias.full_path;
						break;
					}
					else if (textEntered.StartsWith(_alias.alias + "/"))
					{
						String sub = textEntered.Substring(textEntered.IndexOf('/'));
						textEntered = _alias.full_path + sub.Trim('/');
						break;
					}
				}

				processStartInfo.FileName = textEntered;
				processStartInfo.UseShellExecute = true;

				if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
					processStartInfo.Verb = "runas";
                }

				//processStartInfo.Arguments = "/c " + textEntered;
				process.StartInfo = processStartInfo;
				Visibility = Visibility.Hidden;
				try
				{
					process.Start();
				}
				catch (Exception ex)
				{
					//MessageBox.Show(textEntered);
					MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					Visibility = Visibility.Visible;
					Activate();
					Input.Focus();
				}

			}

		}

		private void Load_Plugin(string path)
		{
			Assembly DLL = Assembly.LoadFile(path);

			var objType = DLL.GetType("TestPlugin");

			foreach (var type in DLL.GetTypes())
			{
				if (type != null)
				{

					if (typeof(IPlugin).IsAssignableFrom(type))
					{
						IPlugin plugin = (IPlugin)Activator.CreateInstance(type);

						//var pluginInfo = new PluginInfo(DLL, type);
						plugins.Add(plugin);
					}
				}
			}

		}

		private void Window_Deactivated(object sender, EventArgs e)
		{
			Visibility = Visibility.Hidden;
		}

		private void Window_Activated(object sender, EventArgs e)
		{
			Input.Text = "";
			Input.Focus();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true;
			Hide();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Reload();
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.System && e.OriginalSource is TextBox)
			{
				e.Handled = true;
			}
		}

		private void Reload()
		{
			try
			{
				if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/FLauncher" + "/Aliases.csv"))
				{
					Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/FLauncher");
					using (StreamWriter sw = File.CreateText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/FLauncher" + "/Aliases.csv"))
					{
						sw.WriteLine("alias,full_path");
						sw.WriteLine("g,https://www.google.com/search?q=");
					}
					Reload();
				}
				var reader = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/FLauncher" + "/Aliases.csv");
				var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
				var records = csv.GetRecords<Alias>();

				aliases = records.ToList();



				List<String> dirs = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "*", SearchOption.AllDirectories).ToList();
				dirs.AddRange(Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "*", SearchOption.AllDirectories));

				foreach (String dir in dirs)
				{
					var new_alias = new Alias();
					new_alias.alias = Path.GetFileNameWithoutExtension(dir);
					new_alias.full_path = dir;

					//MessageBox.Show("|" + new_alias.alias + "|" + " " + new_alias.full_path);

					aliases.Add(new_alias);
				}

				foreach (Alias _alias in aliases)
				{
					ComboBoxItem comboBoxItem = new ComboBoxItem();
					comboBoxItem.Content = _alias.alias;
					Input.Items.Add(comboBoxItem);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}


			//Load Plugins

			String[] pluginPaths = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/FLauncher/Plugins");

			foreach (String path in pluginPaths)
			{
				Load_Plugin(path);
			}

			foreach (IPlugin plugin in plugins)
			{
				Options.pluginNames.Add(plugin.name);

			}

			//Load Style
		}
	}
}
