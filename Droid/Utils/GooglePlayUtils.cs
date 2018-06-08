using System;
using Android.Content;
//using Android.Gms.Common;

namespace Rock.Utils
{
    public class GooglePlayUtils
    {
        public static bool IsPlayServicesAvailable(Context context)
		{
            //         int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(context);
            //if (resultCode != ConnectionResult.Success)
            //{
            //	return false;
            //}
            //else
            //{
            //	return true;
            //}
            return true;
		}
    }
}
