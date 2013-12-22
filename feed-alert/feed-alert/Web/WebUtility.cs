namespace feed_alert.Web
{
    using System.IO;
    using System.Web;
    using System.Net;

    class WebUtility
    {
        public static void DownloadFavIcon(string domain, string path)
        {
            DownloadFile(string.Format("http://{0}/favicon.ico", domain), Path.Combine(path, string.Format("{0}.ico", domain)));
        }

        public static void DownloadFile(string url, string file)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (Stream input = response.GetResponseStream())
            using (Stream output = new FileStream(file, FileMode.Create))
            {
                byte[] buf = new byte[1024];
                int bytes;
                while ((bytes = input.Read(buf, 0, 1024)) > 0)
                {
                    output.Write(buf, 0, bytes);
                }
            }
        }

        public static HttpWebResponse MakeHttpRequest(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            return (HttpWebResponse)request.GetResponse();
        }
    }
}
