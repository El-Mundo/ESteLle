using System;
using System.Collections.Generic;
using System.IO;
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
        private BitmapImage aisegImage, aliyunImage, aliyunSkin, aisegSkin, aliyunFace, aisegFace, preview;

        public MainWindow()
        {
            InitializeComponent();
            UserCentre.SetUserConsole(ResultPanel);
        }

        private void SelectingFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Supported Image Files(*.JPG;*.JPEG;*.PNG;*.BMP)|*.JPG;*.JPEG;*.PNG;*.BMP;*.jpg;*.jpeg;*.png;*.bmp|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    PathPanel.Text = filePath;
                    ImagePanel.Source = new BitmapImage(new Uri(filePath));
                }
                catch (Exception ex)
                {
                    UserCentre.ClearConsole();
                    UserCentre.Print("----------------------------------");
                    UserCentre.Print("Cannot open image, " + ex.Message);
                }
            }
        }

        private void Face_Segment(object sender, RoutedEventArgs e)
        {
            UserCentre.ClearConsole();
            UserCentre.Print("Initializing face segmentation...");

            if (string.IsNullOrEmpty(AliyunApplet.instance.bodySegUrl))
            {
                UserCentre.Print("Please use Aliyun Body Segment to generate a body-segment image first.");
            }
            else
            {
                try
                {
                    CallAliyunFaceSegment(AliyunApplet.instance.bodySegUrl, true);
                }
                catch (Exception ex)
                {
                    UserCentre.Print("Cannot segment face.\n" + ex.Message);
                }
            }

            if (AiSegmentApplet.instance == null) return;

            UserCentre.Print("Attempting to process AiSegment image...");

            if (string.IsNullOrEmpty(AiSegmentApplet.instance.imageUrl))
            {
                UserCentre.Print("Please use AiSegment to generate a body-segment image first.");
            }
            else
            {
                try
                {
                    string shanghaiUrl = "";
                    bool converted = AliyunApplet.instance.LocalUriImageToAliyunUrl(AiSegmentApplet.instance.imageUrl, out shanghaiUrl);
                    if (converted)
                    {
                        CallAliyunFaceSegment(shanghaiUrl, false);
                    }
                    else
                    {
                        throw new Exception("Cannot convert AiSegment's result into Aliyun-Shanghai URL.");
                    }
                }
                catch (Exception ex)
                {
                    UserCentre.Print("Cannot segment face.\n" + ex.Message);
                }
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UserCentre.ClearConsole();

            try 
            {
                UpdateDisplayedPreview();

                string imgPath = PathPanel.Text;
                string url = String.Empty;

                try {
                    BitmapImage preview = new BitmapImage();
                    preview.BeginInit();
                    preview.UriSource = new Uri(imgPath);
                    preview.EndInit();
                    UserCentre.WaitBitmapDownloading(preview);
                    
                    if(preview == null)
                    {
                        throw new Exception("The selected file is not an available image!");
                    }
                    /*else if(preview.Width < 32 || preview.Height < 32)
                    {
                        throw new Exception("The processed image must be larger than 32x32!");
                    }
                    else if(preview.Width > 2000 || preview.Height > 2000)
                    {
                        throw new Exception("The selected image must be smaller than 2000x2000!");
                    }*/
                }
                catch(Exception imgEx)
                {
                    throw new Exception("Failed to load image:\n" + imgEx.Message);
                }

                // BMP is not supported by AiSegment but is by Aliyun official APIs
                bool isBmp = imgPath.EndsWith("BMP") || imgPath.EndsWith("bmp");

                bool converted = AliyunApplet.instance.LocalUriImageToAliyunUrl(imgPath, out url);
                if (!converted)
                {
                    throw new Exception("Cannot request Aliyun-Shanghai URL: \n" + url);
                }
                else
                {
                    UserCentre.Print($"Successfully generated URL {url}.\n\nProcessing image...\n");

                    if (AiSegmentApplet.instance != null && !isBmp)
                    {
                        //Do the AiSegment processing here
                        CallAiSegment(imgPath);
                        //Then use Aliyun Segment for an alternative segment result
                        CallAliyunBodySegment(url);
                    }
                    else
                    {
                        //Only do the Aliyun official segment here
                        CallAliyunBodySegment(url);
                        //Clear previous results of AiSegment
                        if(AiSegmentApplet.instance != null) {
                            AiSegmentApplet.instance.imageUrl = "";
                            aisegImage = new BitmapImage();
                            UpdatePreview3(aisegImage);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                UserCentre.Print("----------------------------------");
                UserCentre.Print("Process failed!\n" + ex.Message);
            }
        }

        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            SelectingFile();
        }

        private void FetchImage_Click(object sender, RoutedEventArgs e)
        {
            UpdateDisplayedPreview();
        }

        private void UpdateDisplayedPreview()
        {
            try
            {
                preview = new BitmapImage();
                preview.BeginInit();
                preview.UriSource = new Uri(PathPanel.Text);
                preview.EndInit();
            }
            catch (Exception ex)
            {
                UserCentre.ClearConsole();
                UserCentre.Print("----------------------------------");
                UserCentre.Print("Cannot open image. " + ex.Message);
            }
        }

        private void UpdatePreview2(BitmapImage img)
        {
            try
            {
                ImagePanel2.Source = img;
            }
            catch (Exception ex)
            {
                UserCentre.Print("----------------------------------");
                UserCentre.Print("Cannot load preview at slot 2. " + ex.Message);
            }
        }

        private void UpdatePreview3(BitmapImage img)
        {
            try
            {
                ImagePanel3.Source = img;
            }
            catch (Exception ex)
            {
                UserCentre.Print("----------------------------------");
                UserCentre.Print("Cannot load preview at slot 3. " + ex.Message);
            }
        }

        private void SkinButton2_Click(object sender, RoutedEventArgs e)
        {
            try {
                UserCentre.ClearConsole();

                string name = PathPanel.Text;
                FileInfo file = new FileInfo(name);
                name = file.Name;
                UserCentre.Print("Saving data of " + name + "...");

                if (preview != null)
                {
                    UserCentre.Print("Saving preview...");
                    UserCentre.SaveResult(preview, name, 0);
                }
                if(aliyunImage != null)
                {
                    UserCentre.Print("Saving body...");
                    UserCentre.SaveResult(aliyunImage, name, 1);
                }
                if(aliyunSkin != null)
                {
                    UserCentre.Print("Saving skin...");
                    UserCentre.SaveResult(aliyunSkin, name, 2);
                }
                if(aliyunFace != null)
                {
                    UserCentre.Print("Saving face...");
                    UserCentre.SaveResult(aliyunFace, name, 3);
                }

                if (preview != null)
                {
                    UserCentre.Print("Exporting CSV...");
                    UserCentre.SaveCSV(name, preview, aliyunImage, aliyunSkin, aliyunFace);
                }

                UserCentre.Print("Success");
            }
            catch (Exception ex)
            {
                UserCentre.Print("Failed to save results.\n" + ex.Message);
            }
        }

        private void SkinButton3_Click(object sender, RoutedEventArgs e)
        {
            if (AiSegmentApplet.instance == null) return;

            try
            {
                UserCentre.ClearConsole();

                string name = PathPanel.Text;
                FileInfo file = new FileInfo(name);
                name = file.Name;
                UserCentre.Print("Saving data of " + name + "...");

                if (preview != null)
                {
                    UserCentre.Print("Saving preview...");
                    UserCentre.SaveResult(preview, name, 0);
                }
                if (aisegImage != null)
                {
                    UserCentre.Print("Saving body...");
                    UserCentre.SaveResult(aisegImage, name, 1);
                }
                if (aisegSkin != null)
                {
                    UserCentre.Print("Saving skin...");
                    UserCentre.SaveResult(aisegSkin, name, 2);
                }
                if (aisegFace != null)
                {
                    UserCentre.Print("Saving face...");
                    UserCentre.SaveResult(aisegFace, name, 3);
                }
                
                if(preview != null)
                {
                    UserCentre.Print("Exporting CSV...");
                    UserCentre.SaveCSV(name, preview, aisegImage, aisegSkin, aisegFace);
                }

                UserCentre.Print("Success");
            }
            catch (Exception ex)
            {
                UserCentre.Print("Failed to save results.\n" + ex.Message);
            }
        }

        private void CallAiSegment(string localImagePath)
        {
            BitmapImage image = AiSegmentApplet.instance.RequestImageProcess(localImagePath);
            UserCentre.Print("==============================");
            UserCentre.Print("Genrerating AiSegment preview...");
            aisegImage = image;
            UpdatePreview3(image);
        }

        private void SkinButton_Click(object sender, RoutedEventArgs e)
        {
            UserCentre.ClearConsole();

            if (string.IsNullOrEmpty(AliyunApplet.instance.bodySegUrl))
            {
                UserCentre.Print("Please use Aliyun Body Segment to generate a body-segment image first.");
            }
            else
            {
                try
                {
                    CallAliyunSkinSegment(AliyunApplet.instance.bodySegUrl, true);
                }
                catch (Exception ex)
                {
                    UserCentre.Print("Cannot segment skin.\n" + ex.Message);
                }
            }

            if (AiSegmentApplet.instance == null) return;

            if (string.IsNullOrEmpty(AiSegmentApplet.instance.imageUrl))
            {
                UserCentre.Print("Please use AiSegment to generate a body-segment image first.");
            }
            else
            {
                try
                {
                    string shanghaiUrl = "";
                    bool converted = AliyunApplet.instance.LocalUriImageToAliyunUrl(AiSegmentApplet.instance.imageUrl, out shanghaiUrl);
                    if (converted)
                    {
                        CallAliyunSkinSegment(shanghaiUrl, false);
                    }
                    else
                    {
                        throw new Exception("Cannot convert AiSegment's result into Aliyun-Shanghai URL.");
                    }
                }
                catch (Exception ex)
                {
                    UserCentre.Print("Cannot segment skin.\n" + ex.Message);
                }
            }
        }

        private void CallAliyunBodySegment(string imageUrl)
        {
            BitmapImage image = AliyunApplet.instance.RequestBodySegmentation(imageUrl);
            UserCentre.Print("==============================");
            UserCentre.Print("Genrerating Aliyun Body Segmentation preview...");
            aliyunImage = image;
            UpdatePreview2(image);
        }

        private void CallAliyunSkinSegment(string imageUrl, bool isAliyun)
        {
            UserCentre.Print("Attempting to process skin segmentation...");

            BitmapImage image = AliyunApplet.instance.RequestSkinSegmentation(imageUrl);
            UserCentre.Print("==============================");
            UserCentre.Print("Genrerating Aliyun Skin Segmentation preview...");
            if (isAliyun)
            {
                aliyunSkin = image;
                UpdatePreview2(image);
            }
            else
            {
                aisegSkin = image;
                UpdatePreview3(image);
            }

            UserCentre.Print("Success.");
        }

        private void CallAliyunFaceSegment(string imageUrl, bool isAliyun)
        {
            UserCentre.Print("Attempting to process face segmentation...");

            List<BitmapImage> images = AliyunApplet.instance.RequestFaceSegmentation(imageUrl);
            UserCentre.Print("==============================");
            UserCentre.Print("Genrerating Aliyun Face Segmentation preview...");

            if (images.Count <= 0)
            {
                UserCentre.Print("No face found.");
                return;
            }

            if (isAliyun)
            {
                aliyunFace = images[0];
                UpdatePreview2(images[0]);
            }
            else
            {
                aisegFace = images[0];
                UpdatePreview3(images[0]);
            }

            UserCentre.Print("Success.");
        }

    }
}
