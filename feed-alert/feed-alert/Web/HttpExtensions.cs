using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace feed_alert.Web
{
    using System.IO;
    using System.Net;

    static class HttpExtensions
    {
        public static HttpWebResponse RequestWithUpdateCheck(this HttpWebRequest request, string url, string lastUpdate)
        {
            request.Headers.Add("If-Modified-Since", lastUpdate);
            return (HttpWebResponse)request.GetResponse();
        }

        public static string ReadAllResponseBody(this HttpWebResponse response)
        {
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        public static bool Modified(this HttpWebResponse response)
        {
            return response.StatusCode != HttpStatusCode.NotModified;
        }
    }
}
