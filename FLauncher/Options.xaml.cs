using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FLauncher
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        bool override_dialog = false;
        List<Alias> aliases;

        public Options()
        {
            InitializeComponent();
        }

        public static List<String> pluginNames = new List<string>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
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
                }
                var reader = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/FLauncher" + "/Aliases.csv");
                var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                var records = csv.GetRecords<Alias>();

                aliases = records.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            AliasGrid.ItemsSource = aliases;

            foreach (string name in pluginNames)
            {
                PluginList.Items.Add(name);
            }

            Autoupdate.IsChecked = Settings1.Default.AutoUpdate;
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
            String to_save = "alias,full_path\n";

            foreach (Alias alias in AliasGrid.ItemsSource.Cast<Alias>())
            {
                to_save += alias.alias + "," + alias.full_path + "\n";
            }

            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/FLauncher" + "/Aliases.csv", to_save);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Save_Click(this, new RoutedEventArgs());
        }
    }
}
