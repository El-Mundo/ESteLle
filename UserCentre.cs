using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace EsteLle
{
    public static class UserCentre
    {
        private static TextBox console;

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

    }

}
