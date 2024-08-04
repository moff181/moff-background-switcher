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
        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        const string BackgroundDirectory = @"A:\Background";

        public MainWindow()
        {
            InitializeComponent();

            Hide();

            NotifyIcon ni = new NotifyIcon
            {
                Icon = new Icon("Main.ico"),
                Visible = true,
            };
            ni.DoubleClick += Ni_DoubleClick;

            Task.Run(() =>
            {
                ScheduledRun();
            });
        }

        private void Ni_DoubleClick(object sender, EventArgs e)
        {
            Run();
        }

        private void ScheduledRun()
        {
            while (true)
            {
                Run();

                TimeSpan untilMidnight = DateTime.Today.AddDays(1.0) - DateTime.Now;
                int millis = (int)untilMidnight.TotalMilliseconds;
                Thread.Sleep(millis);

            }
        }

        private void Run()
        {
            string[] files = Directory.GetFiles(BackgroundDirectory, "*.*", SearchOption.AllDirectories);

            string selected = files[new Random().Next(0, files.Length)];

            Set(selected, Style.Stretched);
        }

        private static void Set(string uri, Style style)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            if (style == Style.Stretched)
            {
                key.SetValue(@"WallpaperStyle", 2.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == Style.Centered)
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == Style.Tiled)
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

        private enum Style : int
        {
            Tiled,
            Centered,
            Stretched
        }
    }
}
