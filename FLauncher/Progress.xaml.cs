using System.Windows.Controls;

namespace FLauncher
{
    /// <summary>
    /// Interaction logic for Progress.xaml
    /// </summary>
    public partial class Progress : AdonisUI.Controls.AdonisWindow
    {
        public Progress(string text, string title)
        {
            InitializeComponent();
            Title = title;
            Label.Content = text;
        }

        public void SetPercent(int val)
        {
            Progress1.Value = val;
        }
    }
}
