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
    /// Interaction logic for CopyPasteWindow.xaml
    /// </summary>
    public partial class CopyPasteWindow : AdonisUI.Controls.AdonisWindow
    {
        string CopyText;
        public CopyPasteWindow(string copyText)
        {
            InitializeComponent();
            CopyText = copyText;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CopyBox.AppendText(CopyText);
        }
    }
}
