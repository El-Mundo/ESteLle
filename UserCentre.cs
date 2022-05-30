using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace EsteLle
{
    public static class UserCentre
    {
        private static readonly string BASE_SAVE_PATH = "/ESteLle/base",
            FACE_SAVE_PATH = "/ESteLle/face", SKIN_SAVE_PATH = "/ESteLle/skin", BODY_SAVE_PATH = "/ESteLle/body",
            CSV_PATH = "/ESteLle", CSV_NAME = "data.csv";
        public static readonly string SECRET_PATH = "/ESteLle/secret.csv";
        private const byte ALPHA_THRESHOLD = 8, GRAYSCALE_THRESHOLD = 64;

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

        public static string GetNonExtensionFileName(string name)
        {
            if(!name.Contains('.'))
            {
                return name;
            }
            else
            {
                int dot = name.LastIndexOf('.');
                name = name.Substring(0, dot);
                return name;
            }
        }

        private static void SaveImage(BitmapImage image, string target)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            using (var stream = new FileStream(target, System.IO.FileMode.Create))
            {
                encoder.Save(stream);
            }
        }

        /// <summary>
        /// Save an image result.
        /// </summary>
        /// <param name="image">The image to be saved</param>
        /// <param name="oldName">The original file path</param>
        /// <param name="type">0-base, 1-body, 2-skin, 3-face</param>
        public static void SaveResult(BitmapImage image, string oldName, int type)
        {
            string name = GetNonExtensionFileName(oldName);
            string dir = "";
            switch(type)
            {
                case 0:
                    dir = BASE_SAVE_PATH;
                    break;
                case 1:
                    dir = BODY_SAVE_PATH;
                    break;
                case 2:
                    dir = SKIN_SAVE_PATH;
                    break;
                case 3:
                    dir = FACE_SAVE_PATH;
                    break;
            }
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            string tar = Path.Combine(dir, name + ".png");
            if(File.Exists(tar))
            {
                int i = 0;
                while(File.Exists(Path.Combine(dir, $"{name}({i}).png")) && i < 999)
                {
                    i++;
                }
                tar = Path.Combine(dir, $"{name}({i}).png");
            }
            SaveImage(image, tar);
            FileInfo f = new FileInfo(tar);
            Print("Image saved at: " + f.FullName);
        }

        public static void SaveCSV(string oldName, BitmapImage bas, BitmapImage body, BitmapImage skin, BitmapImage face)
        {
            string name = GetNonExtensionFileName(oldName);
            if(!Directory.Exists(CSV_PATH))
            {
                Directory.CreateDirectory(CSV_PATH);
            }
            string tar = Path.Combine(CSV_PATH, CSV_NAME);
            bool firstLine = false;
            firstLine =(!File.Exists(tar));

            int res = bas.PixelWidth * bas.PixelHeight;
            int bo = GetNonTransparentPixels(body);
            int sk = GetGreyScalePixels(skin);
            int fa = GetNonTransparentPixels(face);

            string line = $"{oldName},{res},{bo},{sk},{fa}\n";

            if(firstLine)
                File.WriteAllText(tar, "Name,Resolution,Body Size,Skin Size,Face Size\n" + line);
            else
                File.AppendAllText(tar, line);

            FileInfo f = new FileInfo(tar);
            Print("CSV saved at: " + f.FullName);
        }

        private static int GetNonTransparentPixels(BitmapImage image)
        {
            if (image == null) return 0;

            int stride = image.PixelWidth * 4;
            int size = image.PixelHeight * stride;
            byte[] pixels = new byte[size];
            image.CopyPixels(pixels, stride, 0);

            int pix = 0;

            for (int x = 0; x < image.PixelWidth; x++)
            {
                for (int y = 0; y < image.PixelHeight; y++)
                {
                    int index = y * stride + 4 * x;

                    byte alpha = pixels[index + 3];

                    if(alpha > ALPHA_THRESHOLD)
                    {
                        pix++;
                    }
                }
            }

            return pix;
        }

        private static int GetGreyScalePixels(BitmapImage image)
        {
            if (image == null) return 0;

            int depth = 1;

            int stride = image.PixelWidth * depth;
            int size = image.PixelHeight * stride;
            byte[] pixels = new byte[size];
            image.CopyPixels(pixels, stride, 0);

            int pix = 0;

            for (int x = 0; x < image.PixelWidth; x++)
            {
                for (int y = 0; y < image.PixelHeight; y++)
                {
                    int index = y * stride + depth * x;

                    byte gray = pixels[index + depth - 1];

                    if (gray > GRAYSCALE_THRESHOLD)
                    {
                        pix++;
                    }
                }
            }

            return pix;
        }

    }

}
