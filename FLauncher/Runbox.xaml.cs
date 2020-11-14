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

namespace FLauncher
{
    /// <summary>
    /// Interaction logic for Runbox.xaml
    /// </summary>
    public partial class Runbox : Window
    {
		List<Alias> aliases;

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
			var reader = new StreamReader(Directory.GetCurrentDirectory() + "/Aliases.csv");
			var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
			var records = csv.GetRecords<Alias>();

			aliases = records.ToList();

			String[] dirs = Directory.GetFiles("C:/ProgramData/Microsoft/Windows/Start Menu/", "*", SearchOption.AllDirectories);

			foreach (String dir in dirs)
            {
				var new_alias = new Alias();
				new_alias.alias = Path.GetFileNameWithoutExtension(dir);
				new_alias.full_path = dir;

				//MessageBox.Show("|" + new_alias.alias + "|" + " " + new_alias.full_path);

				aliases.Add(new_alias);
            }

			//MessageBox.Show("Done");
		}
    }
}
