using System;
using Android.Content;
using Android.Telephony;

namespace WhatMessenger.Droid.Utils
{
    public class PhoneIMEI
    {
        public static string GetImei(Context context)
        {
            var telephonyManager = (TelephonyManager)context.GetSystemService(Context.TelephonyService);
            var id = telephonyManager.Imei;
            if(string.IsNullOrEmpty(id))
            {
                id = string.Empty;
            }
            return id;
        }
    }
}
