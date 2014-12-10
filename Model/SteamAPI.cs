using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Doto_Unlocker.VDF;

namespace Doto_Unlocker.Model
{
    class SteamAPI
    {
        public static string GetSchemaURL(string ApiKey)
        {
            string resp = httpRequest("https://api.steampowered.com/IEconItems_570/GetSchemaURL/v1/?key=" + ApiKey + "&format=vdf&language=en");
            if (resp != null)
            {
                var nodes = VdfParser.Parse(resp);
                return nodes["items_game_url"].Val;
            }
            return null;
        }

        public static VdfNode GetVDFData(string Url)
        {
            string resp = httpRequest(Url);
            return VdfParser.Parse(resp);
        }

        private static String httpRequest(String getUrl)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(getUrl);
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.Headers.Add("Accept-Encoding", "gzip,deflate");
            req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            try
            {
                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                if ((int)response.StatusCode != 200) return null;

                String src = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
                return src;
            }
            catch (WebException)
            {
                return null;
            }
        }
    }
}
