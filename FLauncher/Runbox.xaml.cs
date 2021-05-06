using CsvHelper;
using Microsoft.WindowsAPICodePack.Shell;
using PluginBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MessageBox = AdonisUI.Controls.MessageBox;
using MessageBoxButton = AdonisUI.Controls.MessageBoxButton;
using MessageBoxImage = AdonisUI.Controls.MessageBoxImage;
using Path = System.IO.Path;

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

        string textEntered = "";
        string args = "";

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                if (ParamsBox.Visibility == Visibility.Hidden)
                {
                    Height = 119;
                    ParamsBox.Visibility = Visibility.Visible;
                    ParamsBox.Text = "";
                }
                else
                {
                    Height = 70;
                    ParamsBox.Visibility = Visibility.Hidden;
                }

            }
            if (e.Key == Key.Enter)
            {
                textEntered = Input.Text.ToLower();
                args = ParamsBox.Text;


                bool inputHandled = false;

                foreach (IPlugin plugin in plugins)
                {
                    try
                    {
                        inputHandled = plugin.CommandEntered(textEntered, ParamsBox.Text);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error");
                    }
                }

                if (inputHandled == true)
                {
                    return;
                }

                Process process = new Process();
                ProcessStartInfo processStartInfo = new ProcessStartInfo();

                foreach (Alias _alias in aliases)
                {
                    //MessageBox.Show(_alias.alias + " " +_alias.full_path);
                    if (textEntered == _alias.alias.ToLower())
                    {
                        textEntered = _alias.full_path;
                        if (args == "")
                            args = _alias.parameters;
                        break;
                    }
                    else if (textEntered.StartsWith(_alias.alias.ToLower() + "/"))
                    {
                        string sub = textEntered.Substring(textEntered.IndexOf('/'));
                        textEntered = _alias.full_path + sub.Trim('/');
                        break;
                    }
                }

                processStartInfo.FileName = textEntered;
                processStartInfo.UseShellExecute = true;
                processStartInfo.Arguments = args;
                processStartInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

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

                    Height = 70;
                    ParamsBox.Visibility = Visibility.Hidden;

                    ParamsBox.Text = "";
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
            Assembly DLL;

            try
            {
                DLL = Assembly.LoadFile(path);
            }
            catch
            {
                return;
            }

            //var objType = DLL.GetType("TestPlugin");

            foreach (var type in DLL.GetTypes())
            {
                if (type != null)
                {

                    if (typeof(IPlugin).IsAssignableFrom(type))
                    {
                        IPlugin plugin = (IPlugin)Activator.CreateInstance(type);

                        //var pluginInfo = new PluginInfo(DLL, type);
                        if(!plugins.Contains(plugin))
                        {
                            plugins.Add(plugin);
                        }
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

            Height = 70;
            ParamsBox.Visibility = Visibility.Hidden;

            ParamsBox.Text = "";

            Reload();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Reload();

            String[] pluginPaths = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/FLauncher/Plugins");

            foreach (String path in pluginPaths)
            {
                Load_Plugin(path);
            }

            foreach (IPlugin plugin in plugins)
            {
                Options.pluginNames.Add(plugin.name);

            }
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
                Input.Items.Clear();
                aliases = new List<Alias>();
                var FODLERID_AppsFolder = new Guid("{1e87508d-89c2-42f0-8a7e-645a0f50ca58}");
                ShellObject appsFolder = (ShellObject)KnownFolderHelper.FromKnownFolderId(FODLERID_AppsFolder);

                foreach (var app in (IKnownFolder)appsFolder)
                {
                    // The friendly app name
                    string name = app.Name;
                    // The ParsingName property is the AppUserModelID
                    string appUserModelID = app.ParsingName; // or app.Properties.System.AppUserModel.ID
                                                             // You can even get the Jumbo icon in one shot
                                                             //ImageSource icon = app.Thumbnail.ExtraLargeBitmapSource;

                    var new_alias = new Alias
                    {
                        alias = name,
                        full_path = "explorer.exe",
                        parameters = @"shell:appsFolder\" + appUserModelID
                    };


                    aliases.Add(new_alias);
                }

                List<String> searchDirs = new List<string>()
                {
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu),
                    Environment.GetFolderPath(Environment.SpecialFolder.StartMenu)
                };
                List<String> shortcuts = new List<string>();

                foreach(String dir in searchDirs)
                {
                    shortcuts.AddRange(Directory.GetFiles(dir, "*", SearchOption.AllDirectories));
                }

                foreach (String shortcut in shortcuts)
                {
                    var new_alias = new Alias
                    {
                        alias = Path.GetFileNameWithoutExtension(shortcut),
                        full_path = shortcut
                    };

                    //MessageBox.Show("|" + new_alias.alias + "|" + " " + new_alias.full_path);
                    bool exists = false;
                    foreach (var _alias in aliases)
                    {
                        if (_alias.alias == new_alias.alias)
                        {
                            exists = true;
                            _alias.alias = new_alias.alias;
                            _alias.full_path = new_alias.full_path;
                            _alias.parameters = "";

                            //MessageBox.Show(_alias.alias);
                        }
                    }

                    if (exists == false)
                        aliases.Add(new_alias);
                }

                aliases.Sort((x, y) => string.Compare(y.alias, x.alias));

                var reader = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/FLauncher" + "/Aliases.csv");
                var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                var records = csv.GetRecords<Alias>();
                var recordlist = records.ToList();

                aliases.AddRange(recordlist);

                aliases.Reverse();

                foreach (Alias _alias in aliases)
                {
                    if (_alias.alias != "" && !_alias.alias.ToLower().StartsWith("uninstall"))

                    {
                        ComboBoxItem comboBoxItem = new ComboBoxItem();
                        if (_alias.alias.Length > 21)
                        {
                            comboBoxItem.Content = _alias.alias;//.Substring(0, 18) + "...";
                        }
                        else
                        {
                            comboBoxItem.Content = _alias.alias;

                        }
                        comboBoxItem.ToolTip = _alias.alias;
                        Input.Items.Add(comboBoxItem);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
    }
}
