using System.Text.Json;
using System.Windows;

namespace FLauncher
{
    /// <summary>
    /// Interaction logic for PluginMetadataCreate.xaml
    /// </summary>
    public partial class PluginMetadataCreate : AdonisUI.Controls.AdonisWindow
    {
        public PluginMetadataCreate()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var pInfo = new PluginInfo();
            pInfo.Name = pName.Text;
            pInfo.Description = pDesc.Text;
            pInfo.DownloadUrl = pUrl.Text;
            pInfo.Version = pVersion.Text;
            CreateJson(pInfo);
        }

        void CreateJson(PluginInfo pluginInfo)
        {
            string jsonString = JsonSerializer.Serialize(pluginInfo);
            new CopyPasteWindow(jsonString).Show();
        }
    }
}
