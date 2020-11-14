using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GlobalHotKey;

namespace FLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

    }

    /// <summary>
    /// A simple command that displays the command parameter as
    /// a dialog message.
    /// </summary>
    public class ShowMessageCommand : ICommand
	{
		HotKeyManager hkManager = new HotKeyManager();

        private void HkManager_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            if(e.HotKey.Key == Key.Space)
            {
				Show();
            }
        }

        Runbox r = new Runbox();
		public void Execute(object parameter)
		{
			if (parameter.ToString() == "ShowWindow")
			{
                try
                {
					var hotkey = hkManager.Register(Key.Space, ModifierKeys.Alt);
					hkManager.KeyPressed += HkManager_KeyPressed;
				}
                catch { }

				Show();
			}
			if(parameter.ToString() == "Quit")
            {
				hkManager.Dispose();
				Application.Current.Shutdown();
            }
		}
		  
		public bool CanExecute(object parameter)
		{
			return true;
		}

		public event EventHandler CanExecuteChanged;

		void Show()
        {
			if(r.Visibility == Visibility.Visible)
            {
				r.Visibility = Visibility.Hidden;
			}
			else
            {
				r.Visibility = Visibility.Visible;
				r.Activate();
			}
		}
	}

	public class Alias
	{
		public string alias { get; set; }
		public string full_path { get; set; }
	}
}
