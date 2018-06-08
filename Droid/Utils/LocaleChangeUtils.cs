using System;
using Android.Annotation;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Preferences;
using Java.Util;

namespace Rock.Utils
{
    public class LocaleChangeUtils
    {
        private const string SELECTED_LANGUAGE = "Locale.Helper.Selected.Language";

        public static Android.Content.Context onAttach(Context context)
        {
            String lang = getPersistedData(context, Locale.Default.Language);
            return setLocale(context, lang);
        }

        public static Context onAttach(Context context, String defaultLanguage)
        {
            String lang = getPersistedData(context, defaultLanguage);
            return setLocale(context, lang);
        }

        public static String getLanguage(Context context)
        {
            return getPersistedData(context, Locale.Default.Language);
        }

        public static Context setLocale(Context context, String language)
        {
            persist(context, language);

            if ((int)Build.VERSION.SdkInt >= 24)
            {
                return updateResources(context, language);
            }

            return updateResourcesLegacy(context, language);
        }

        private static String getPersistedData(Context context, String defaultLanguage)
        {
            ISharedPreferences preferences = PreferenceManager.GetDefaultSharedPreferences(context);
            return preferences.GetString(SELECTED_LANGUAGE, defaultLanguage);
        }

        private static void persist(Context context, String language)
        {
            var preferences = PreferenceManager.GetDefaultSharedPreferences(context);
            ISharedPreferencesEditor editor = preferences.Edit();
            editor.PutString(SELECTED_LANGUAGE, language);
            editor.Apply();
        }

        [TargetApi(Value=24)]
        private static Context updateResources(Context context, String language)
        {
            Locale locale = new Locale(language);
            Locale.Default = locale;

            Configuration configuration = context.Resources.Configuration;
            configuration.SetLocale(locale);
            configuration.SetLayoutDirection(locale);

            return context.CreateConfigurationContext(configuration);
        }

        private static Context updateResourcesLegacy(Context context, String language)
        {
            Locale locale = new Locale(language);
            Locale.Default = locale;
            Resources resources = context.Resources;
            Configuration configuration = resources.Configuration;
            configuration.Locale = locale;
            if ((int)Build.VERSION.SdkInt >= 17)
            {
                configuration.SetLayoutDirection(locale);
            }
            resources.UpdateConfiguration(configuration, resources.DisplayMetrics);
            return context;
        }
    }
}
