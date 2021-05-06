using System.Windows;

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
