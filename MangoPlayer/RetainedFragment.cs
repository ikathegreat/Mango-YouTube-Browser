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

namespace MangoPlayer
{
    public class RetainedFragment : Fragment
    {
        private StateData data;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
        }

        public void setData(StateData data)
        {
            this.data = data;
        }

        public StateData getData()
        {
            return data;
        }
    }
}