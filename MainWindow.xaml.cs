using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace MoffBackgroundSwitcher
{
    public partial class MainWindow : Window
    {
        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDWININICHANGE = 0x02;

        private readonly Settings _settings;

        private SettingsWindow _settingsWindow;

        public MainWindow()
        {
            InitializeComponent();

            Hide();

            _settings = Settings.Load();

            var ni = new NotifyIcon
            {
                Icon = new Icon("Main.ico"),
                Visible = true,
                Text = "Moff Background Switcher"
            };
            ni.DoubleClick += NextItem_Event;
            
            ni.ContextMenu = CreateContextMenu();

            Task.Run(() =>
            {
                ScheduledRun();
            });
        }

        private ContextMenu CreateContextMenu()
        {
            var contextMenu = new ContextMenu();

            var nextItem = new MenuItem
            {
                Index = 0,
                Text = "Next",
            };
            nextItem.Click += NextItem_Event;
            contextMenu.MenuItems.Add(nextItem);

            var settingsItem = new MenuItem
            {
                Index = 1,
                Text = "Settings",
            };
            settingsItem.Click += OpenSettings_Event;
            contextMenu.MenuItems.Add(settingsItem);

            return contextMenu;
        }

        private void OpenSettings_Event(object sender, EventArgs e)
        {
            if(_settingsWindow != null)
            {
                return;
            }

            _settingsWindow = new SettingsWindow(_settings);
            _settingsWindow.Closed += (s, ev) => _settingsWindow = null;
            _settingsWindow.Show();
        }

        private void NextItem_Event(object sender, EventArgs e)
        {
            NextItem();
        }

        private void ScheduledRun()
        {
            while (true)
            {
                NextItem();

                TimeSpan untilMidnight = DateTime.Today.AddDays(1.0) - DateTime.Now;
                int millis = (int)untilMidnight.TotalMilliseconds;
                Thread.Sleep(millis);
            }
        }

        private void NextItem()
        {
            if(!Directory.Exists(_settings.BackgroundDirectory))
            {
                return;
            }

            string[] files = Directory.GetFiles(_settings.BackgroundDirectory, "*.*", SearchOption.AllDirectories);

            if(files.Length == 0)
            {
                return;
            }

            string selected = files[new Random().Next(0, files.Length)];

            Set(selected, WallpaperStyle.Stretched);
        }

        private static void Set(string uri, WallpaperStyle style)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            if (style == WallpaperStyle.Stretched)
            {
                key.SetValue(@"WallpaperStyle", 2.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == WallpaperStyle.Centered)
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == WallpaperStyle.Tiled)
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 1.ToString());
            }

            SystemParametersInfo(
                SPI_SETDESKWALLPAPER,
                0,
                uri,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private enum WallpaperStyle : int
        {
            Tiled,
            Centered,
            Stretched
        }
    }
}
