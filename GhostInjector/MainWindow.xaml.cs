using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

  

        public MainWindow()
        {
            InitializeComponent();
        }

        public SolidColorBrush BrushFromHex(string hexColorString)
        {
            return (SolidColorBrush)(new BrushConverter().ConvertFrom(hexColorString));
        }


        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Github_MouseEnter(object sender, MouseEventArgs e)
        {
            HomeButtonBorder.Background = BrushFromHex("#FF111111");
        }

        private void Github_MouseLeave(object sender, MouseEventArgs e)
        {
           HomeButtonBorder.Background = BrushFromHex("#FF060606");
        }

        private void Dev_MouseEnter(object sender, MouseEventArgs e)
        {
            DevButtonBorder.Background = BrushFromHex("#FF111111");
        }

        private void Dev_MouseLeave(object sender, MouseEventArgs e)
        {
            DevButtonBorder.Background = BrushFromHex("#FF060606");
        }

        private void SettingsIcon_MouseEnter(object sender, MouseEventArgs e)
        {
            SettingsButtonBorder.Background = BrushFromHex("#FF111111");
        }

        private void SettingsIcon_MouseLeave(object sender, MouseEventArgs e)
        {
            SettingsButtonBorder.Background = BrushFromHex("#FF060606");
        }

        private void Github_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Body.Content = new Home();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Body.Content = new Home();
        }

        private void Dev_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Body.Content = new Dev();
        }


    }


}
