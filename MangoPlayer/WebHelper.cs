using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HtmlAgilityPack;

namespace MangoPlayer
{
    public static class WebHelper
    {
        public static List<string> FetchVideoList(string url)
        {
            List<string> youtubeUrlList = new List<string>();

            try
            {
                var web = new HtmlWeb();
                var doc = web.Load(url);

                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//@src"))
                {
                    HtmlAttribute att = link.Attributes["src"];
                    youtubeUrlList.Add("https://" + att.Value.Remove(0, 6)); //remove preceeding //www.
                }
                youtubeUrlList = youtubeUrlList.Where(x => x.Contains("youtube.com")).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return youtubeUrlList;
        }
    }
}