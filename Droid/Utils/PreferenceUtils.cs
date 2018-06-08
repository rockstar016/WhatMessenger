using System;
using Android.Content;
using Android.Preferences;

namespace TaskManagerBLM.Droid.Sources.Utils
{
    public class PreferenceUtils
    {
        public const string REMEMBER_ME = "REMEMBER";
        public const string TOKEN = "TOKEN";
        public const string FBTOKEN = "FBTOKEN";
        public const string EMAIL = "EMAIL";
        public const string LANG = "Lang";
        public const string WALLPAPER = "WallPaper";
        public static void saveBool(Context context, string key, bool remember)
        {
			ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
			ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean(key, remember);
            editor.Commit();
        }

        public static bool readBool(Context context, string key)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            bool retVal = prefs.GetBoolean(key, false);
            return retVal;
        }

		public static void saveString(Context context, string key, string valueStr)
		{
			ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
			ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString(key, valueStr);
			editor.Commit();
		}

		public static string readString(Context context, string key)
		{
			ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            string retVal = prefs.GetString(key, "");
			return retVal;
		}
    }
}
