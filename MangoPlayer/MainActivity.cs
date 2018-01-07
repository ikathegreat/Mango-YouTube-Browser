using Android.App;
using Android.Widget;
using Android.OS;
using Android.Webkit;
using Android.Views;
using HtmlAgilityPack;
using System.Linq;
using System.Collections.Generic;
using System;

namespace MangoPlayer
{
    [Activity(Label = "Mango Player", MainLauncher = true, Icon = "@mipmap/ic_add_to_queue_black_48dp")]
    public class MainActivity : Activity
    {
        //Button refreshButton;
        Button nextButton;
        IMenuItem browseMode_latest;
        IMenuItem browseMode_topAllTime;
        IMenuItem browseMode_topYear;
        IMenuItem browseMode_topMonth;
        IMenuItem browseMode_topWeek;

        int currentPageIndex = 1;
        int currentVidIndex = 1;
        List<string> curPageVideoList = new List<string>();

        string fullFeedUrl = string.Empty;

        const string feedUrlPrefix = "http://bestofyoutube.com/index.php?page=";
        const string feedUrlSuffix = "&show=";

        const string feedTopSuffixAllTime = "alltime";
        const string feedTopSuffixYear = "year";
        const string feedTopSuffixMonth = "month";
        const string feedTopSuffixWeek = "week";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            debugAlertMessage("onafter_OnCreate");

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            debugAlertMessage ("onafter_SetContentView");

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

            //Toolbar will now take on default actionbar characteristics
            SetActionBar(toolbar);
            if (ActionBar != null)
                ActionBar.Title = "Latest";
            debugAlertMessage("onafter_SetActionBar");

            // Get our button from the layout resource,
            // and attach an event to it
            //refreshButton = FindViewById<Button>(Resource.Id.refreshButton);
            nextButton = FindViewById<Button>(Resource.Id.nextButton);
            WebView localWebView = FindViewById<WebView>(Resource.Id.LocalWebView);
            localWebView.Settings.JavaScriptEnabled = true;

            debugAlertMessage("onafter_findLocalWebView");

            //refreshButton.Click += delegate
            //{
            //    refresh();
            //};
            nextButton.Click += delegate
            {
                next();
            };

            //Latest
            fullFeedUrl = feedUrlPrefix + currentPageIndex;
            

            refresh();
            debugAlertMessage("onafter_refresh");
        }
        private void debugAlertMessage(string message, string title = "debug")
        {

            //AlertDialog.Builder dialog = new AlertDialog.Builder(this);
            //AlertDialog alert = dialog.Create();
            //alert.SetTitle(title);
            //alert.SetMessage(message);
            //alert.SetButton("OK", (c, ev) =>
            //{
            //    // Ok button click task  
            //});
            //alert.Show();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.home, menu);

            browseMode_latest = menu.FindItem(Resource.Id.menu_browseMode_latest);
            browseMode_topAllTime = menu.FindItem(Resource.Id.menu_browseMode_topAllTime);
            browseMode_topYear = menu.FindItem(Resource.Id.menu_browseMode_topYear);
            browseMode_topMonth = menu.FindItem(Resource.Id.menu_browseMode_topMonth);
            browseMode_topWeek = menu.FindItem(Resource.Id.menu_browseMode_topWeek);

            browseMode_latest.SetCheckable(true);
            browseMode_topAllTime.SetCheckable(true);
            browseMode_topYear.SetCheckable(true);
            browseMode_topMonth.SetCheckable(true);
            browseMode_topWeek.SetCheckable(true);

            browseMode_latest.SetChecked(true);

            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // Toast.MakeText(this, item.TitleFormatted, ToastLength.Short).Show();

            if (item.TitleFormatted.ToString() == "Refresh")
            {
                refresh();
                return base.OnOptionsItemSelected(item);
            }

            if (item.TitleFormatted.ToString() == "Latest")
            {
                fullFeedUrl = feedUrlPrefix + currentPageIndex;
                ActionBar.Title = "Latest";

                browseMode_latest.SetChecked(true);
                browseMode_topAllTime.SetChecked(false);
                browseMode_topYear.SetChecked(false);
                browseMode_topMonth.SetChecked(false);
                browseMode_topWeek.SetChecked(false);

                refresh();
            }
            else if (item.TitleFormatted.ToString() == "Top - All Time")
            {
                fullFeedUrl = feedUrlPrefix + currentPageIndex + feedUrlSuffix + feedTopSuffixAllTime;
                ActionBar.Title = "Top - All Time";

                browseMode_latest.SetChecked(false);
                browseMode_topAllTime.SetChecked(true);
                browseMode_topYear.SetChecked(false);
                browseMode_topMonth.SetChecked(false);
                browseMode_topWeek.SetChecked(false);

                refresh();
            }
            else if (item.TitleFormatted.ToString() == "Top - Year")
            {
                fullFeedUrl = feedUrlPrefix + currentPageIndex + feedUrlSuffix + feedTopSuffixYear;
                ActionBar.Title = "Top - Year";

                browseMode_latest.SetChecked(false);
                browseMode_topAllTime.SetChecked(false);
                browseMode_topYear.SetChecked(true);
                browseMode_topMonth.SetChecked(false);
                browseMode_topWeek.SetChecked(false);

                refresh();
            }
            else if (item.TitleFormatted.ToString() == "Top - Month")
            {
                fullFeedUrl = feedUrlPrefix + currentPageIndex + feedUrlSuffix + feedTopSuffixMonth;
                ActionBar.Title = "Top - Month";

                browseMode_latest.SetChecked(false);
                browseMode_topAllTime.SetChecked(false);
                browseMode_topYear.SetChecked(false);
                browseMode_topMonth.SetChecked(true);
                browseMode_topWeek.SetChecked(false);

                refresh();

            }
            else if (item.TitleFormatted.ToString() == "Top - Week")
            {
                fullFeedUrl = feedUrlPrefix + currentPageIndex + feedUrlSuffix + feedTopSuffixWeek;
                ActionBar.Title = "Top - Week";

                browseMode_latest.SetChecked(false);
                browseMode_topAllTime.SetChecked(false);
                browseMode_topYear.SetChecked(false);
                browseMode_topMonth.SetChecked(false);
                browseMode_topWeek.SetChecked(true);

                refresh();
            }

            return base.OnOptionsItemSelected(item);
        }

        void next()
        {
            currentVidIndex++;
            if (currentVidIndex > curPageVideoList.Count)
            {
                currentPageIndex++;
                currentVidIndex = 1;
                refresh();
            }
            else
            {

                WebView localWebView = FindViewById<WebView>(Resource.Id.LocalWebView);
                localWebView.SetWebViewClient(new WebViewClient());
                localWebView.LoadUrl(curPageVideoList[currentVidIndex - 1]);
            }
        }

        void refresh()
        {
            //refreshButton.Enabled = false;
            try
            {
                string urlToGet = fullFeedUrl;
                curPageVideoList = fetchBOYTPageVideos(urlToGet);

                WebView localWebView = FindViewById<WebView>(Resource.Id.LocalWebView);
                localWebView.SetWebViewClient(new WebViewClient());
                localWebView.LoadUrl(curPageVideoList[currentVidIndex - 1]);
            }
            finally
            {
                //refreshButton.Enabled = true;
            }
        }

        List<string> fetchBOYTPageVideos(string url)
        {
            List<string> youtubeUrlList = new List<string>();

            var web = new HtmlWeb();
            var doc = web.Load(url);

            if (doc.ParseErrors != null && doc.ParseErrors.Count() > 0)
            {
                // Handle any parse errors as required
            }
            else
            {
                youtubeUrlList = new List<string>();

                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//@src"))
                {
                    HtmlAttribute att = link.Attributes["src"];
                    youtubeUrlList.Add("https://" + att.Value.Remove(0, 6)); //remove preceeding //www.
                }
                youtubeUrlList = youtubeUrlList.Where(x => x.Contains("youtube.com")).ToList();

            }
            return youtubeUrlList;
        }
    }
}

