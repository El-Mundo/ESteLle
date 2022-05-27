using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace EsteLle
{
    public class AiSegmentApplet
    {
        private static readonly string QUERY_URL = "https://aliapi.aisegment.com/segment/matting";

        public static AiSegmentApplet instance;

        private string appCode;
        /// <summary>
        /// The last image processed by AiSegment.
        /// </summary>
        public string imageUrl;

        public AiSegmentApplet(string appCode)
        {
            this.appCode = appCode;
        }

        /// <summary>
        /// Request to process the image in AiSegment.
        /// </summary>
        /// <param name="imageLocalPath">The local position of the image to be processed.</param>
        /// <returns>The processed image as BitmapImage.</returns>
        public BitmapImage RequestImageProcess(string imageLocalPath)
        {
            try {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(QUERY_URL));
                HttpWebResponse httpResponse = null;
                httpRequest.Method = "POST";
                httpRequest.Headers.Add("Authorization", "APPCODE " + appCode);
                httpRequest.ContentType = "application/json; charset=UTF-8";

                AiSegmentQueryBody qBody = new AiSegmentQueryBody(imageLocalPath);
                string queryBody = JsonConvert.SerializeObject(qBody);

                byte[] data = Encoding.UTF8.GetBytes(queryBody);
                using (Stream stream = httpRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                try
                {
                    httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                }
                catch (WebException ex)
                {
                    httpResponse = (HttpWebResponse)ex.Response;
                }

                UserCentre.Print("AiSegment Status: " + httpResponse.StatusCode.ToString());
                UserCentre.Print("AiSegment Query Method: " + httpResponse.Method);
                //UserCentre.Print("AiSegment Query Header: " + httpResponse.Headers.ToString());
                UserCentre.Print("AiSegment Result: =================================");
                Stream st = httpResponse.GetResponseStream();
                StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
                string result = reader.ReadToEnd();
                UserCentre.Print(result);
                UserCentre.Print("===================== END OF RESULT =====================");

                AiSegmentResult qResult = JsonConvert.DeserializeObject<AiSegmentResult>(result);
                BitmapImage output = new BitmapImage();

                imageUrl = qResult.data.result;

                output.BeginInit();
                output.UriSource = new Uri(imageUrl);
                output.EndInit();
                UserCentre.WaitBitmapDownloading(output);
                return output;
            }
            catch (Exception e)
            {
                UserCentre.Print("================================");
                UserCentre.Print("AiSegment Warning:\n" + e.Message);
                return null;
            }
        }

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

    }

    [System.Serializable]
    class AiSegmentQueryBody
    {
        /// <summary>
        /// "jpg" or "png"
        /// </summary>
        public string type;
        /// <summary>
        /// BASE64 encoded image, must be a jpg, jpeg, or png.
        /// </summary>
        public string photo;
        /// <summary>
        /// If this is 1, the image will not be processed unless a face is found.
        /// Default as 0 (recommended).
        /// </summary>
        public int face_required;
        /// <summary>
        /// Whther the transparent area after segmentation should be cropped.
        /// </summary>
        public int is_crop_content;

        public AiSegmentQueryBody(string imageLocalPath)
        {
            if(!File.Exists(imageLocalPath))
            {
                throw new Exception("Cannot load the image to be converted to BASE64.");
            }

            if (imageLocalPath.EndsWith(".png") || imageLocalPath.EndsWith(".PNG"))
            {
                type = "png";
            }
            else
            {
                type = "jpg";
            }

            byte[] imgBytes = File.ReadAllBytes(imageLocalPath);
            photo = Convert.ToBase64String(imgBytes);

            face_required = 0;
            is_crop_content = 0;
        }

    }

    [System.Serializable]
    class AiSegmentResult
    {
        public int status;
        public AiSegmentData data;
    }

    [System.Serializable]
    class AiSegmentData
    {
        public string result;
    }

}
