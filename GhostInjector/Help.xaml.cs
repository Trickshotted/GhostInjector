using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GhostInjector
{
    /// <summary>
    /// Interaction logic for Help.xaml
    /// </summary>
    public partial class Help : Page
    {
        public Help()
        {
            InitializeComponent();
        }

        public SolidColorBrush BrushFromHex(string hexColorString)
        {
            return (SolidColorBrush)(new BrushConverter().ConvertFrom(hexColorString));
        }

        private void Label_MouseEnter(object sender, MouseEventArgs e)
        {
            OpenButton.Background = BrushFromHex("#FF0F1EA8");
            OpenButton.BorderBrush = BrushFromHex("#FF0F1EA8");
        }

        private void Label_MouseLeave(object sender, MouseEventArgs e)
        {
            OpenButton.Background = BrushFromHex("#FF1024DA");
            OpenButton.BorderBrush = BrushFromHex("#FF1024DA");
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://discord.gg/6zcNKM9j3r");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            WebClient wc = new WebClient();
            string[] data = wc.DownloadString("https://ghostinjectorinfo.w3spaces.com/").Split('~');
            MainInfo.Text = data[2];
        }
    }
}
