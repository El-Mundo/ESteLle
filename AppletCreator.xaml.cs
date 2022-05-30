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

namespace EsteLle
{
    /// <summary>
    /// AppletCreator.xaml 的交互逻辑
    /// </summary>
    public partial class AppletCreator : Window
    {
        public AppletCreator()
        {
            InitializeComponent();
            UserCentre.SetUserConsole(UserConsole);

            if(System.IO.File.Exists(UserCentre.SECRET_PATH))
            {
                try
                {
                    string con = System.IO.File.ReadAllText(UserCentre.SECRET_PATH);
                    string[] line = con.Split(',');
                    AccessKey.Text = line[0];
                    AccessSecret.Text = line[1];
                    AppCode.Text = line[2];
                }
                catch (Exception e)
                {
                    System.IO.FileInfo f = new System.IO.FileInfo(UserCentre.SECRET_PATH);
                    UserCentre.Print("Saved keys corrupted: " + f.FullName + "\n" + e.Message);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string key = AccessKey.Text;
            string sec = AccessSecret.Text;
            UserCentre.ClearConsole();
            if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(sec))
            {
                try 
                {
                    AliyunApplet.instance = new AliyunApplet(AccessKey.Text, AccessSecret.Text);
                }
                catch(Exception ex)
                {
                    UserCentre.Print("Invalid Aliyun account. " + ex.Message);
                    return;
                }
            }
            else
            {
                UserCentre.Print("-------------------------------------------------------");
                UserCentre.Print("Please input the access key and access secret of Aliyun.");
                return;
            }

            string code = AppCode.Text;
            if (!string.IsNullOrWhiteSpace(code))
            {
                AiSegmentApplet.instance = new AiSegmentApplet(code);
            }

            string oss = OSSBucket.Text;
            if(!string.IsNullOrWhiteSpace(oss))
            {
                try
                {
                    AliyunApplet.instance.SetOSSEndpoint("oss-cn-shanghai.aliyuncs.com", oss);
                }
                catch(Exception exx)
                {
                    UserCentre.Print("Invalid Aliyun OSS account. " + exx.Message);
                    return;
                }
            }

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

    }
}
