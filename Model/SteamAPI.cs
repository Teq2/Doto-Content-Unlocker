/*
 * Doto-Content-Unlocker, tool for unlocking custom dota2 content.
 * 
 *  Copyright (c) 2014 Teq, https://github.com/Teq2/Doto-Content-Unlocker
 * 
 *  Permission is hereby granted, free of charge, to any person obtaining a copy of 
 *  this software and associated documentation files (the "Software"), to deal in the 
 *  Software without restriction, including without limitation the rights to use, copy, 
 *  modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
 *  and to permit persons to whom the Software is furnished to do so, subject to the 
 *  following conditions:
 * 
 *  The above copyright notice and this permission notice shall be included in 
 *  all copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 *  INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
 *  PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 *  HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
 *  CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
 *  OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

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
