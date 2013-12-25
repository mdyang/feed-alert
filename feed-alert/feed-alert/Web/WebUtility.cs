namespace feed_alert.Web
{
    using System;
    using System.IO;
    using System.Net;
    using System.ServiceModel.Syndication;
    using System.Web;
    using System.Xml;

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

        public static HttpWebResponse MakeHttpRequest(string url, string lastModified = null)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            if (lastModified != null)
            {
                request.IfModifiedSince = DateTime.Parse(lastModified);
            }

            try
            {
                return (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                if (ex.Response != null && ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotModified)
                {
                    return null;
                }
                throw ex;
            }
        }

        public static SyndicationFeed ReadFeed(HttpWebResponse response)
        {
            using (XmlReader xmlReader = XmlReader.Create(response.GetResponseStream()))
            {
                return SyndicationFeed.Load(xmlReader);
            }
        }
    }
}
