using System;

using Android.App;
using Android.OS;
using Android.Runtime;

using Plugin.CurrentActivity;
using TaskManagerBLM.Droid.Sources.Utils;
using WhatMessenger.Model.Auth;
using WhatMessenger.Model.BaseModel;
using WhatMessenger.ViewModel;
using WhatMessengerStringResource;

namespace WhatMessenger.Droid
{
    //You can specify additional application information in this attribute
    [Application]
    public class MainApplication : Application, Application.IActivityLifecycleCallbacks
    {
        public LanguageInterface CurrentLangSetting;
        public UserDTO Me { get; set; }
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
        : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            RegisterActivityLifecycleCallbacks(this);
            App.Initialize();
            string currentRegion = PreferenceUtils.readString(this.ApplicationContext, PreferenceUtils.LANG);
            InitLangResource(currentRegion);
        }

        public void InitLangResource(string CurrentLang)
        {
            if (string.IsNullOrEmpty(CurrentLang))
            {
                CurrentLangSetting = ChineseStringResouce.GetInstance();
            }
            else if (string.Equals(CurrentLang, "CH"))
            {
                CurrentLangSetting = ChineseStringResouce.GetInstance();
            }
            else if(string.Equals(CurrentLang,"EN"))
            {
                CurrentLangSetting = EnglishStringResource.GetInstance();
            }
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityDestroyed(Activity activity)
        {

        }

        public void OnActivityPaused(Activity activity)
        {

        }

        public void OnActivityResumed(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {

        }

        public void OnActivityStarted(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityStopped(Activity activity)
        {

        }
    }
}
