using System;
using System.IO;
using System.Net;

namespace WebChecker.Model
{
    public static class Status
    {
        public static string GetStatus(string url)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.CreateHttp(url);
                request.Method = "GET";
                var response = (HttpWebResponse)request.GetResponse();
                return response.StatusCode.ToString();
            }
            catch
            {
                return "Address not responding";
            }
        }

        public static Site GetInfo(Site site)
        {
                System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                HttpWebRequest request = (HttpWebRequest)WebRequest.CreateHttp(site.address);
                request.Method = "HEAD";
                request.KeepAlive = false;
                
                try
                {
                    timer.Start();
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    timer.Stop();
                    site.status = response.StatusCode.ToString();
                    response.Dispose();
                }
                catch(WebException e)
                {
                    using (var stream = e.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        site.status = reader.ReadToEnd();
                    }
                }
                catch(Exception)
                {
                    site.status = "Address not responding";
                }
                site.responseTime = timer.Elapsed.Milliseconds;
                return site;
            }
        }
}
