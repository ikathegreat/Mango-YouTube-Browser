﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MangoPlayer
{
    public static class Constants
    {
        public const byte Key = 0x53;

        public const string feedUrlPrefix = "OycnI2kMDDE2ICc8NSo8JicmMTZ9MDw+DDo9NzYrfSM7I2wjMjQ2";//"http://bestofyoutube.com/index.php?page=";
        public const string feedUrlSuffix = "&show=";

        public const string feedTopSuffixAllTime = "alltime";
        public const string feedTopSuffixYear = "year";
        public const string feedTopSuffixMonth = "month";
        public const string feedTopSuffixWeek = "week";

        public static readonly string TAG_RETAINED_FRAGMENT = "RetainedFragment";

        public enum BrowseMode { Latest, AllTime, Year, Month, Week };
    }
}