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
    /// Interaction logic for Progress.xaml
    /// </summary>
    public partial class Progress : Window
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
