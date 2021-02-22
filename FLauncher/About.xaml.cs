using System;
using System.Collections.Generic;
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
    }
}
