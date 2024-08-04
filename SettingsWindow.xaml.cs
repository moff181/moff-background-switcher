using System.Windows;

namespace MoffBackgroundSwitcher
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly Settings _settings;

        public SettingsWindow(Settings settings)
        {
            InitializeComponent();
            _settings = settings;

            BackgroundDirectoryInput.Text = settings.BackgroundDirectory;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _settings.BackgroundDirectory = BackgroundDirectoryInput.Text;
            _settings.Save();
        }
    }
}
