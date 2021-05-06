using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;

namespace FLauncher
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : AdonisUI.Controls.AdonisWindow
    {
        public About()
        {
            InitializeComponent();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var p = new System.Diagnostics.Process();
            p.StartInfo.FileName = (sender as Hyperlink).NavigateUri.ToString();
            p.StartInfo.UseShellExecute = true;
            p.Start();
        }

        private void AdonisWindow_Activated(object sender, EventArgs e)
        {
            VersionLabel.Content = "Version: " + File.ReadAllText(Directory.GetCurrentDirectory() + @"\version.txt");
        }
    }
}
