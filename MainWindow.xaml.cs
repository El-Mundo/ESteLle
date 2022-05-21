using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AlibabaCloud.SDK.VIAPI.Utils;
using Microsoft.Win32;

namespace EsteLle
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

        private void SelectFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try 
            { 
                ImagePanel.Source = new BitmapImage(new Uri(PathPanel.Text));
            }
            catch(Exception ex)
            {
                ResultPanel.Text = "Cannot open image, " + ex.Message;
            }
        }
    }
}
