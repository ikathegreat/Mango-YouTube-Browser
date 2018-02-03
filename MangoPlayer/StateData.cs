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
using static MangoPlayer.Constants;

namespace MangoPlayer
{
   public class StateData
    {
        /// <summary>
        /// Representation of current application settings or variables
        /// to be saved to and retained in a Fragment
        /// </summary>
        public StateData()
        {
            currentPageIndex = 1;
            currentVideoIndex = 0;
            currentPageVideoList = new List<string>();
            browseMode = BrowseMode.Latest;
            fullFeedUrl = feedUrlPrefix + currentPageIndex;
        }

        private int currentPageIndex;
        private int currentVideoIndex;
        private List<string> currentPageVideoList;
        private BrowseMode browseMode;
        private string fullFeedUrl;

        public string FullFeedUrl
        {
            get { return fullFeedUrl; }
            set { fullFeedUrl = value; }
        }

        public BrowseMode BrowseMode
        {
            get { return browseMode; }
            set { browseMode = value; }
        }

        public List<string> CurrentPageVideoList
        {
            get { return currentPageVideoList; }
            set { currentPageVideoList = value; }
        }

        public int CurrentVideoIndex
        {
            get { return currentVideoIndex; }
            set { currentVideoIndex = value; }
        }        
        
        public int CurrentPageIndex
        {
            get { return currentPageIndex; }
            set { currentPageIndex = value; }
        }

    }
}