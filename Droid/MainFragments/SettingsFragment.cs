
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using API.Models.RequestModels;
using Rock.Utils;
using TaskManagerBLM.Droid.Sources.Utils;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Droid.Engine;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.Fragments
{
    public class SettingsFragment : MainBaseFragment
    {
        public static SettingsFragment GetInstance() => new SettingsFragment { Arguments = new Bundle() };

        Button btChatWallPaper, btNotification, btClearAll, btLang;
        BottomSheetDialog mBottomSheetDialog;
        ChatListViewModel PrivateChatViewModel;
        GroupListViewModel GroupChatViewModel;
        ProfileViewModel MyProfileViewModel;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            PrivateChatViewModel = EngineService.EngineInstance.ChatListViewModel;
            GroupChatViewModel = EngineService.EngineInstance.GroupListViewModel;
            MyProfileViewModel = EngineService.EngineInstance.ProfileViewModel;

            var rootView = inflater.Inflate(Resource.Layout.fragment_setting, container, false);
            btChatWallPaper = rootView.FindViewById<Button>(Resource.Id.btChatWallPaper);
            //btNotification = rootView.FindViewById<Button>(Resource.Id.btNotification);
            btClearAll = rootView.FindViewById<Button>(Resource.Id.btClearAll);
            btLang = rootView.FindViewById<Button>(Resource.Id.btChangeLang);

            btChatWallPaper.Click += BtChatWallPaper_Click;
            //btNotification.Click += BtNotification_Click;
            btClearAll.Click += BtClearAll_Click;
            btLang.Click += BtLang_Click;

            InitViewLangs();
            return rootView;
        }

        void InitViewLangs()
        {
            var StringResource = ParentActivity.MyApplication.CurrentLangSetting.GetStringResourceContents();
            btChatWallPaper.Text = StringResource.GetValueOrDefault("changeWallPaper");
            btClearAll.Text = StringResource.GetValueOrDefault("clearChat");
            btLang.Text = StringResource.GetValueOrDefault("changeLang");
        }

        void BtClearAll_Click(object sender, EventArgs e)
        {
            if(Plugin.Connectivity.CrossConnectivity.Current.IsConnected)
            {
                var token = ParentActivity.MyApplication.Me.TOKEN;
                PrivateChatViewModel.ClearAllChatCommand.Execute(token);
                GroupChatViewModel.ClearAllMessageHistoryCommand.Execute(token);    
            }
            else
            {
                DialogUtils.ShowOKDialog(ParentActivity, @"Warning", @"No Internet Connection");
            }
        }

        public override void OnStart()
        {
            base.OnStart();
            if(MyProfileViewModel == null) 
            {
                EngineService.EngineInstance.ProfileViewModel = new ProfileViewModel();
                MyProfileViewModel = EngineService.EngineInstance.ProfileViewModel;
            }
            if (MyProfileViewModel.ME == null)
                MyProfileViewModel.ME = ParentActivity.MyApplication.Me;
            PrivateChatViewModel.PropertyChanged += PrivateChatViewModel_PropertyChanged;
            GroupChatViewModel.PropertyChanged += GroupChatViewModel_PropertyChanged;
            MyProfileViewModel.PropertyChanged += MyProfileViewModel_PropertyChanged;
        }

        public override void OnStop()
        {
            base.OnStop();
            PrivateChatViewModel.PropertyChanged -= PrivateChatViewModel_PropertyChanged;
            GroupChatViewModel.PropertyChanged -= GroupChatViewModel_PropertyChanged;
            MyProfileViewModel.PropertyChanged -= MyProfileViewModel_PropertyChanged;
        }

        void MyProfileViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(string.Equals(e.PropertyName, nameof(ProfileViewModel.IsBusy)))
            {
                if(MyProfileViewModel.IsBusy)
                {
                    ShowLoadingDialog("Update Language");    
                }
                else
                {
                    HideLoadingDialog();
                }
            }

            if(string.Equals(e.PropertyName, nameof(ProfileViewModel.ME)))
            {
                this.ParentActivity.RunOnUiThread(() =>
                {
                    ParentActivity.MyApplication.Me = MyProfileViewModel.ME;
                    ParentActivity.ChangeLanguage();
                });
            }
        }


        void PrivateChatViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(string.Equals(e.PropertyName, nameof(ChatListViewModel.IsBusy)))
            {
                if(PrivateChatViewModel.IsBusy && !GroupChatViewModel.IsBusy)
                {
                    ShowLoadingDialog("Clear Histories");
                }
                else if(!PrivateChatViewModel.IsBusy && !GroupChatViewModel.IsBusy)
                {
                    HideLoadingDialog();
                }
            }
        }

        void GroupChatViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, nameof(GroupListViewModel.IsBusy)))
            {
                if (GroupChatViewModel.IsBusy && !PrivateChatViewModel.IsBusy)
                {
                    ShowLoadingDialog("Clear Histories");
                }
                else if (!PrivateChatViewModel.IsBusy && !GroupChatViewModel.IsBusy)
                {
                    HideLoadingDialog();
                }
            }
        }

        #region Language Change Screen
        void BtLang_Click(object sender, EventArgs e)
        {
            mBottomSheetDialog = new BottomSheetDialog(this.Activity);
            View sheetView = ParentActivity.LayoutInflater.Inflate(Resource.Layout.fragment_change_lang, null);
            mBottomSheetDialog.SetContentView(sheetView);
            InitSettingDialog(sheetView);

            mBottomSheetDialog.Show();
        }

        void InitSettingDialog(View rootView)
        {
            var chineseRadio = rootView.FindViewById<AppCompatRadioButton>(Resource.Id.radioChinese);
            var englishRadio = rootView.FindViewById<AppCompatRadioButton>(Resource.Id.radioEnglish);
            string CurrentLang = PreferenceUtils.readString(this.ParentActivity, PreferenceUtils.LANG);
            if (string.Equals(CurrentLang, @"EN"))
            {
                englishRadio.Checked = true;
            }
            else
            {
                chineseRadio.Checked = true;
            }

            chineseRadio.Click += ChineseRadio_Click;
            englishRadio.Click += EnglishRadio_Click;
        }

        private void EnglishRadio_Click(object sender, EventArgs e)
        {
            mBottomSheetDialog.Hide();
            UpdateLang(true);
        }

        private void ChineseRadio_Click(object sender, EventArgs e)
        {
            mBottomSheetDialog.Hide();
            UpdateLang(false);
        }

        void UpdateLang(bool IsEnglish)
        {
            PreferenceUtils.saveString(ParentActivity, PreferenceUtils.LANG, IsEnglish?"EN":"CH");
            if (Plugin.Connectivity.CrossConnectivity.Current.IsConnected)
            {
                var langVal = IsEnglish ? 0 : 1;
                var request = new GetProfileRequest() { TOKEN = ParentActivity.MyApplication.Me.TOKEN, USERID = Convert.ToString(langVal)};
                MyProfileViewModel.CommandUpdateLang.Execute(request);
            }
            else
            {
                DialogUtils.ShowOKDialog(ParentActivity, @"Warning", @"No Internet Connection");
            }
        }
        #endregion

        #region Wallpaper region
        void BtChatWallPaper_Click(object sender, EventArgs e)
        {
            mBottomSheetDialog = new BottomSheetDialog(this.Activity);
            View sheetView = ParentActivity.LayoutInflater.Inflate(Resource.Layout.fragment_setting_chat_background, null);
            mBottomSheetDialog.SetContentView(sheetView);
            InitWallpaperDialog(sheetView);

            mBottomSheetDialog.Show();
        }

        void InitWallpaperDialog(View rootView)
        {
            var flowerRadio = rootView.FindViewById<AppCompatRadioButton>(Resource.Id.radio1);
            var nightRadio = rootView.FindViewById<AppCompatRadioButton>(Resource.Id.radio2);
            var CurrentWall = PreferenceUtils.readBool(this.ParentActivity, PreferenceUtils.WALLPAPER);
            flowerRadio.Checked = !CurrentWall;
            nightRadio.Checked = CurrentWall;
            flowerRadio.Click += FlowerRadio_Click;
            nightRadio.Click += NightRadio_Click;

        }

        private void NightRadio_Click(object sender, EventArgs e)
        {
            PreferenceUtils.saveBool(this.ParentActivity, PreferenceUtils.WALLPAPER, true);
            mBottomSheetDialog.Dismiss();
        }

        private void FlowerRadio_Click(object sender, EventArgs e)
        {
            PreferenceUtils.saveBool(this.ParentActivity, PreferenceUtils.WALLPAPER, false);
            mBottomSheetDialog.Dismiss();
        }

        #endregion
    }
}
