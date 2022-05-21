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
        }

        private void Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            AliyunApplet.instance = new AliyunApplet(AccessKey.Text, AccessSecret.Text);
        }
    }
}
