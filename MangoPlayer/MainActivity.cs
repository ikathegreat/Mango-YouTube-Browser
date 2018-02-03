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
using System.Threading.Tasks;
using static MangoPlayer.Constants;

namespace MangoPlayer
{
    [Activity(Label = "Mango Player", MainLauncher = true, Icon = "@mipmap/ic_add_to_queue_black_48dp")]
    public class MainActivity : Activity
    {
        bool showDebugMessage = false;

        Button nextButton;
        IMenuItem browseMode_latest;
        IMenuItem browseMode_topAllTime;
        IMenuItem browseMode_topYear;
        IMenuItem browseMode_topMonth;
        IMenuItem browseMode_topWeek;

        private RetainedFragment mRetainedFragment;
        private ShareActionProvider mShareActionProvider;
        private StateData stateData = new StateData();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

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

            nextButton = FindViewById<Button>(Resource.Id.nextButton);
            WebView localWebView = FindViewById<WebView>(Resource.Id.LocalWebView);
            localWebView.Settings.JavaScriptEnabled = true;

            debugAlertMessage("onafter_findLocalWebView");

            nextButton.Click += delegate { navigateToNextVideo(); };

            // find the retained fragment on activity restarts
            mRetainedFragment = (RetainedFragment)FragmentManager.FindFragmentByTag(TAG_RETAINED_FRAGMENT);
            if (mRetainedFragment != null)
            {
                stateData = mRetainedFragment.getData();
                loadWebViewUrl();
            }
            else
            {
                //First time startup
                refresh();
            }

            debugAlertMessage("onafter_refresh");

            this.Window.AddFlags(WindowManagerFlags.Fullscreen);
            //this.Window.ClearFlags(WindowManagerFlags.Fullscreen); //this restores the notification bar
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
                if (mRetainedFragment != null)
                    FragmentManager.BeginTransaction().Remove(mRetainedFragment).Commit();

            }
        }

        private void debugAlertMessage(string message, string title = "Debug Message")
        {
            if (!showDebugMessage)
                return;

            AlertDialog.Builder dialog = new AlertDialog.Builder(this);
            AlertDialog alert = dialog.Create();
            alert.SetTitle(title);
            alert.SetMessage(message);
            alert.SetButton("OK", (c, ev) =>
            {
                // Ok button click task  
            });
            alert.Show();
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

        private void openCurVideoInYouTube()
        {
            var uri = Android.Net.Uri.Parse(stateData.CurrentPageVideoList[stateData.CurrentVideoIndex]);
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.TitleFormatted.ToString() == Resources.GetString(Resource.String.Refresh))
                refresh(true);

            if (item.TitleFormatted.ToString() == Resources.GetString(Resource.String.OpenInYouTubeApp))
                openCurVideoInYouTube();

            if (item.TitleFormatted.ToString() == Resources.GetString(Resource.String.Share))
            {
                Intent intentsend = new Intent();
                intentsend.SetAction(Intent.ActionSend);
                intentsend.PutExtra(Intent.ExtraText, stateData.CurrentPageVideoList[stateData.CurrentVideoIndex]);
                intentsend.SetType("text/plain");
                StartActivity(intentsend); //This does Just One or Always prompt
            }

            if (item.TitleFormatted.ToString() == Resources.GetString(Resource.String.BrowseMode_Latest))
            {
                ActionBar.Title = Resources.GetString(Resource.String.BrowseMode_Latest);
                updateBrowseMode(BrowseMode.Latest);
            }
            else if (item.TitleFormatted.ToString() == Resources.GetString(Resource.String.BrowseMode_TopAllTime))
            {
                ActionBar.Title = Resources.GetString(Resource.String.BrowseMode_TopAllTime);
                updateBrowseMode(BrowseMode.AllTime);
            }
            else if (item.TitleFormatted.ToString() == Resources.GetString(Resource.String.BrowseMode_TopYear))
            {
                ActionBar.Title = Resources.GetString(Resource.String.BrowseMode_TopYear);
                updateBrowseMode(BrowseMode.Year);
            }
            else if (item.TitleFormatted.ToString() == Resources.GetString(Resource.String.BrowseMode_TopMonth))
            {
                ActionBar.Title = Resources.GetString(Resource.String.BrowseMode_TopMonth);
                updateBrowseMode(BrowseMode.Month);
            }
            else if (item.TitleFormatted.ToString() == Resources.GetString(Resource.String.BrowseMode_TopWeek))
            {
                ActionBar.Title = Resources.GetString(Resource.String.BrowseMode_TopWeek);
                updateBrowseMode(BrowseMode.Week);
            }

            return base.OnOptionsItemSelected(item);
        }

        private void updateBrowseMode(BrowseMode browseMode)
        {
            stateData.BrowseMode = browseMode;
            browseMode_latest.SetChecked(browseMode == BrowseMode.Latest);
            browseMode_topAllTime.SetChecked(browseMode == BrowseMode.AllTime);
            browseMode_topYear.SetChecked(browseMode == BrowseMode.Year);
            browseMode_topMonth.SetChecked(browseMode == BrowseMode.Month);
            browseMode_topWeek.SetChecked(browseMode == BrowseMode.Week);
            refresh();
        }

        void navigateToNextVideo()
        {
            stateData.CurrentVideoIndex++;
            if (stateData.CurrentVideoIndex > stateData.CurrentPageVideoList.Count - 1)
            {
                stateData.CurrentPageIndex++;
                stateData.CurrentVideoIndex = 0;
                loadWebViewUrl();
            }
            else
            {
                WebView localWebView = FindViewById<WebView>(Resource.Id.LocalWebView);
                localWebView.SetWebViewClient(new WebViewClient());
                string url = stateData.CurrentPageVideoList[stateData.CurrentVideoIndex];

                localWebView.LoadUrl(url);
            }
        }

        void refresh(bool showToast = false)
        {
            stateData.CurrentPageIndex = 1;
            stateData.CurrentVideoIndex = 0;

            loadWebViewUrl();
            if (showToast)
                RunOnUiThread(() => Toast.MakeText(this, Resources.GetString(Resource.String.Refreshed), ToastLength.Long).Show());
        }

        private void loadWebViewUrl()
        {
            switch (stateData.BrowseMode)
            {
                case BrowseMode.Latest:
                    stateData.FullFeedUrl = feedUrlPrefix + stateData.CurrentPageIndex;
                    break;
                case BrowseMode.AllTime:
                    stateData.FullFeedUrl = feedUrlPrefix + stateData.CurrentPageIndex + feedUrlSuffix + feedTopSuffixAllTime;
                    break;
                case BrowseMode.Year:
                    stateData.FullFeedUrl = feedUrlPrefix + stateData.CurrentPageIndex + feedUrlSuffix + feedTopSuffixYear;
                    break;
                case BrowseMode.Month:
                    stateData.FullFeedUrl = feedUrlPrefix + stateData.CurrentPageIndex + feedUrlSuffix + feedTopSuffixMonth;
                    break;
                case BrowseMode.Week:
                    stateData.FullFeedUrl = feedUrlPrefix + stateData.CurrentPageIndex + feedUrlSuffix + feedTopSuffixWeek;
                    break;
                default:
                    stateData.FullFeedUrl = feedUrlPrefix + stateData.CurrentPageIndex;
                    break;
            }

            string urlToGet = stateData.FullFeedUrl;
            stateData.CurrentPageVideoList = WebHelper.FetchVideoList(urlToGet);

            WebView localWebView = FindViewById<WebView>(Resource.Id.LocalWebView);
            localWebView.SetWebViewClient(new WebViewClient());
            localWebView.LoadUrl(stateData.CurrentPageVideoList[stateData.CurrentVideoIndex]);
        }


    }
}

