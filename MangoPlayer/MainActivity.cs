using Android.App;
using Android.Widget;
using Android.OS;
using Android.Webkit;
using Android.Views;
using HtmlAgilityPack;
using System.Linq;
using System.Collections.Generic;
using System;
using Android.Content;
using Com.Cloudrail.SI;
using Com.Cloudrail.SI.Services;
using System.Threading.Tasks;
using static MangoPlayer.Constants;

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

        //IMenuItem option_autoPlay;

        //int stateData.CurrentPageIndex = 1;
        //int stateData.CurrentVideoIndex = 1;
        //List<string> stateData.CurrentPageVideoList = new List<string>();

        //string stateData.FullFeedUrl = string.Empty;


        //const string feedUrlPrefix = "http://bestofyoutube.com/index.php?page=";
        //const string feedUrlSuffix = "&show=";

        //const string feedTopSuffixAllTime = "alltime";
        //const string feedTopSuffixYear = "year";
        //const string feedTopSuffixMonth = "month";
        //const string feedTopSuffixWeek = "week";

        //BrowseMode browseMode = BrowseMode.Latest;

        //enum BrowseMode { Latest, AllTime, Year, Month, Week };

        private static readonly string TAG_RETAINED_FRAGMENT = "RetainedFragment";
        private RetainedFragment mRetainedFragment;

        private StateData stateData = new StateData();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //retrieveSet();

            debugAlertMessage("onafter_OnCreate");

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            debugAlertMessage("onafter_SetContentView");

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

            // find the retained fragment on activity restarts
            mRetainedFragment = (RetainedFragment)FragmentManager.FindFragmentByTag(TAG_RETAINED_FRAGMENT);
            if (mRetainedFragment != null)
            {
                stateData = mRetainedFragment.getData();
            }
            refresh();

            debugAlertMessage("onafter_refresh");

            this.Window.AddFlags(WindowManagerFlags.Fullscreen);
            //this.Window.ClearFlags(WindowManagerFlags.Fullscreen);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            mRetainedFragment = (RetainedFragment)FragmentManager.FindFragmentByTag(TAG_RETAINED_FRAGMENT);

            // create the fragment and data the first time
            if (mRetainedFragment == null)
            {
                // add the fragment
                mRetainedFragment = new RetainedFragment();
                FragmentManager.BeginTransaction().Add(mRetainedFragment, TAG_RETAINED_FRAGMENT).Commit();
                // load data from a data source or perform any calculation
                mRetainedFragment.setData(stateData);
            }
            else
            {
                mRetainedFragment.setData(stateData);
            }

            base.OnSaveInstanceState(outState);
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (IsFinishing)
            {
                // we will not need this fragment anymore, this may also be a good place to signal
                // to the retained fragment object to perform its own cleanup.
                FragmentManager.BeginTransaction().Remove(mRetainedFragment).Commit();

            }
        }

        protected override void OnDestroy()
        {
            //saveSet();
            base.OnDestroy();
        }
        protected void saveSet()
        {

            //store
            //ISharedPreferences prefs = Application.Context.GetSharedPreferences("MangoPlayer", FileCreationMode.Private);
            //ISharedPreferencesEditor prefEditor = prefs.Edit();
            //prefEditor.PutBoolean("startupDone", startupDone);
            //prefEditor.Commit();

        }

        // Function called from OnCreate
        protected void retrieveSet()
        {
            //retreive 
            ISharedPreferences prefs = Application.Context.GetSharedPreferences("MangoPlayer", FileCreationMode.Private);
            bool somePref = prefs.GetBoolean("startupDone", false);

            //Show a toast
            //string test = startupDone ? "true" : "false";
            //RunOnUiThread(() => Toast.MakeText(this, test, ToastLength.Long).Show());

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

            //option_autoPlay = menu.FindItem(Resource.Id.menu_AutoPlay);

            browseMode_latest.SetCheckable(true);
            browseMode_topAllTime.SetCheckable(true);
            browseMode_topYear.SetCheckable(true);
            browseMode_topMonth.SetCheckable(true);
            browseMode_topWeek.SetCheckable(true);

            //option_autoPlay.SetCheckable(true);

            browseMode_latest.SetChecked(true);

            return base.OnCreateOptionsMenu(menu);
        }

        private void openCurVideoInYouTube()
        {

            var uri = Android.Net.Uri.Parse(stateData.CurrentPageVideoList[stateData.CurrentVideoIndex]);
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // Toast.MakeText(this, item.TitleFormatted, ToastLength.Short).Show();

            if (item.TitleFormatted.ToString() == "Refresh")
            {
                refresh(true);
                return base.OnOptionsItemSelected(item);
            }

            if (item.TitleFormatted.ToString() == "Open in YouTube")
            {
                openCurVideoInYouTube();
                return base.OnOptionsItemSelected(item);
            }

            if (item.TitleFormatted.ToString() == "Share")
            {
                Intent intentsend = new Intent();
                intentsend.SetAction(Intent.ActionSend);
                intentsend.PutExtra(Intent.ExtraText, stateData.CurrentPageVideoList[stateData.CurrentVideoIndex]);
                intentsend.SetType("text/plain");
                StartActivity(intentsend);
                return base.OnOptionsItemSelected(item);
            }

            if (item.TitleFormatted.ToString() == "Auto Play")
            {
                //autoPlay = !autoPlay;
                //option_autoPlay.SetChecked(autoPlay);
                return base.OnOptionsItemSelected(item);
            }

            if (item.TitleFormatted.ToString() == "Latest")
            {
                stateData.FullFeedUrl = feedUrlPrefix + stateData.CurrentPageIndex;
                ActionBar.Title = "Latest";

                browseMode_latest.SetChecked(true);
                browseMode_topAllTime.SetChecked(false);
                browseMode_topYear.SetChecked(false);
                browseMode_topMonth.SetChecked(false);
                browseMode_topWeek.SetChecked(false);
                stateData.BrowseMode = BrowseMode.Latest;
                refresh();
            }
            else if (item.TitleFormatted.ToString() == "Top - All Time")
            {
                ActionBar.Title = "Top - All Time";

                browseMode_latest.SetChecked(false);
                browseMode_topAllTime.SetChecked(true);
                browseMode_topYear.SetChecked(false);
                browseMode_topMonth.SetChecked(false);
                browseMode_topWeek.SetChecked(false);
                stateData.BrowseMode = BrowseMode.AllTime;

                refresh();
            }
            else if (item.TitleFormatted.ToString() == "Top - Year")
            {
                ActionBar.Title = "Top - Year";

                browseMode_latest.SetChecked(false);
                browseMode_topAllTime.SetChecked(false);
                browseMode_topYear.SetChecked(true);
                browseMode_topMonth.SetChecked(false);
                browseMode_topWeek.SetChecked(false);
                stateData.BrowseMode = BrowseMode.Year;

                refresh();
            }
            else if (item.TitleFormatted.ToString() == "Top - Month")
            {
                ActionBar.Title = "Top - Month";

                browseMode_latest.SetChecked(false);
                browseMode_topAllTime.SetChecked(false);
                browseMode_topYear.SetChecked(false);
                browseMode_topMonth.SetChecked(true);
                browseMode_topWeek.SetChecked(false);
                stateData.BrowseMode = BrowseMode.Month;

                refresh();

            }
            else if (item.TitleFormatted.ToString() == "Top - Week")
            {
                ActionBar.Title = "Top - Week";

                browseMode_latest.SetChecked(false);
                browseMode_topAllTime.SetChecked(false);
                browseMode_topYear.SetChecked(false);
                browseMode_topMonth.SetChecked(false);
                browseMode_topWeek.SetChecked(true);
                stateData.BrowseMode = BrowseMode.Week;

                refresh();
            }

            return base.OnOptionsItemSelected(item);
        }

        void next()
        {
            stateData.CurrentVideoIndex++;
            if (stateData.CurrentVideoIndex > stateData.CurrentPageVideoList.Count)
            {
                stateData.CurrentPageIndex++;
                stateData.CurrentVideoIndex = 0;
                refresh();
            }
            else
            {

                WebView localWebView = FindViewById<WebView>(Resource.Id.LocalWebView);
                localWebView.SetWebViewClient(new WebViewClient());
                string url = stateData.CurrentPageVideoList[stateData.CurrentVideoIndex];

                //if (autoPlay)
                //    url += "?rel=0&autoplay=1";

                localWebView.LoadUrl(url);
            }
        }

        void refresh(bool showToast = false)
        {
            //refreshButton.Enabled = false;
            stateData.CurrentPageIndex = 1;
            stateData.CurrentVideoIndex = 0;

            try
            {
                if (stateData.BrowseMode == BrowseMode.Latest)
                {
                    stateData.FullFeedUrl = feedUrlPrefix + stateData.CurrentPageIndex;
                }
                else if (stateData.BrowseMode == BrowseMode.AllTime)
                {

                    stateData.FullFeedUrl = feedUrlPrefix + stateData.CurrentPageIndex + feedUrlSuffix + feedTopSuffixAllTime;
                }
                else if (stateData.BrowseMode == BrowseMode.Year)
                {
                    stateData.FullFeedUrl = feedUrlPrefix + stateData.CurrentPageIndex + feedUrlSuffix + feedTopSuffixYear;
                }
                else if (stateData.BrowseMode == BrowseMode.Month)
                {
                    stateData.FullFeedUrl = feedUrlPrefix + stateData.CurrentPageIndex + feedUrlSuffix + feedTopSuffixMonth;
                }
                else if (stateData.BrowseMode == BrowseMode.Week)
                {
                    stateData.FullFeedUrl = feedUrlPrefix + stateData.CurrentPageIndex + feedUrlSuffix + feedTopSuffixWeek;
                }

                string urlToGet = stateData.FullFeedUrl;
                stateData.CurrentPageVideoList = fetchBOYTPageVideos(urlToGet);

                WebView localWebView = FindViewById<WebView>(Resource.Id.LocalWebView);
                localWebView.SetWebViewClient(new WebViewClient());
                localWebView.LoadUrl(stateData.CurrentPageVideoList[stateData.CurrentVideoIndex]);

                //youtubeApiTest();
            }
            finally
            {
                if (showToast)
                    RunOnUiThread(() => Toast.MakeText(this, "Refreshed", ToastLength.Long).Show());
            }
        }

        private void youtubeApiTest()
        {
            var t = Task.Run(() =>
            {
                CloudRail.AppKey = "5a726594ff0b7e2f2f9ce356";

                YouTube service = new YouTube(
                    this,
                    "656256027781-r7m8eqmkoo2rekjn2lnqdu9ac02vg2nj.apps.googleusercontent.com",
                    "",
                    "com.cloudrail.example:/auth",
                    "someState"
                );

                // This service requires the advanced authentication method. Visit the tutorials page
                // to learn more about the different authentication methods available.
                service.UseAdvancedAuthentication();

                service.Login();
            });
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

