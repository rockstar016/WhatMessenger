
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using WhatMessenger.Droid.AuthActivity;

namespace WhatMessenger.Droid.Bases
{
    public class RegisterBaseFragment : Android.Support.V4.App.Fragment
    {
        protected RegisterActvity ParentActivity;
        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            ParentActivity = context as RegisterActvity;
        }

        public override void OnDetach()
        {
            base.OnDetach();
            ParentActivity = null;
        }
    }
}
