using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace EsteLle
{
    public static class UserCentre
    {
        private static TextBox console;

        /// <summary>
        /// Image downloading will be seen as failed if it is still downloading after 10 seconds.
        /// </summary>
        private const int SLEEP_TIMEOUT = 300;

        public static void SetUserConsole(TextBox console)
        {
            UserCentre.console = console;
        }

        public static void Print(string message)
        {
            console.Text += "\n" + message;
        }

        public static void ClearConsole()
        {
            console.Text = "";
        }

        public static void WaitBitmapDownloading(BitmapImage image)
        {
            /*int sleep = 0;
            while(image.IsDownloading)
            {
                Thread.Sleep(32);
                sleep++;
                if(sleep > SLEEP_TIMEOUT)
                {
                    Print("Image loading timeout!");
                    return;
                }
            }*/
            return;
        }

    }

}
