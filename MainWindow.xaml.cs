using System;
using System.Collections.Generic;
using System.Linq;
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

        public void GenerateURL()
        {
            string accessKeyId = "LTAI5t9Bv7zv7WsvuCXvbYac";   // 您的AccessKeyID
            string accessSecret = "euPYCDVgMSQwMTZgUnoYDaqDCJwKUQ";   // 您的AccessKeySecret
            string imageUrl = PathPanel.Text;  // 上传成功后，返回上传后的文件地址
            string result = "";
            try 
            { 
                FileUtils fileobj = FileUtils.getInstance(accessKeyId, accessSecret);
                result = fileobj.Upload(imageUrl);
            }
            catch(Exception e)
            {
                ResultPanel.Text = e.Message;
            }
            Console.WriteLine(result);
            ResultPanel.Text = result;
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
            GenerateURL();
        }
    }
}
