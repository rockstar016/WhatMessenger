
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using API.Models.RequestModels;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Droid.Engine;
using WhatMessenger.Model.BaseModel;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.Account
{
    [Activity(Label = "PrivacyActivity")]
    public class PrivacyActivity : BaseActivity
    {
        ProfileViewModel MyProfileViewModel;
        ContactListViewModel MyContactListViewModel;

        protected override int LayoutResource => Resource.Layout.activity_privacy;
        BottomSheetDialog mBottomSheetDialog_ProfilePhoto, mBottomSheetDialog_StatusPhoto;

        TextView txtWhoCanSee, txtMessaging;

        TextView txtProfilePhotoIndicator, txtProfilePhoto;
        Button btProfilePhoto;

        TextView txtStatusIndicator, txtStatus;
        Button btStatus;

        TextView txtBlockedContactIndicator, txtBlockedContact;
        Button btBlocked;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            MyProfileViewModel = EngineService.EngineInstance.ProfileViewModel;
            MyContactListViewModel = EngineService.EngineInstance.ContactListViewModel;

            base.OnCreate(savedInstanceState);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            //SupportActionBar.Title = "Privacy";
            txtWhoCanSee = FindViewById<TextView>(Resource.Id.txtWhoCanSee);
            txtMessaging = FindViewById<TextView>(Resource.Id.txtMessaging);

            txtProfilePhotoIndicator = FindViewById<TextView>(Resource.Id.txtProfilePhotoIndicator);
            txtProfilePhoto = FindViewById<TextView>(Resource.Id.txtProfilePhoto);
            txtStatusIndicator = FindViewById<TextView>(Resource.Id.txtStatusIndicator);
            txtStatus = FindViewById<TextView>(Resource.Id.txtStatus);
            txtBlockedContactIndicator = FindViewById<TextView>(Resource.Id.txtBlockedContactIndicator);
            txtBlockedContact = FindViewById<TextView>(Resource.Id.txtBlockedContact);

            btProfilePhoto = FindViewById<Button>(Resource.Id.btProfilePhoto);
            btStatus = FindViewById<Button>(Resource.Id.btStatus);
            btBlocked = FindViewById<Button>(Resource.Id.btBlocked);

            btProfilePhoto.Click += BtProfilePhoto_Click;
            btStatus.Click += BtStatus_Click;
            btBlocked.Click += BtBlocked_Click;
        }

		protected override void OnStart()
		{
            base.OnStart();
            if(MyProfileViewModel == null)
            {
                EngineService.EngineInstance.ProfileViewModel = new ProfileViewModel();
                MyProfileViewModel = EngineService.EngineInstance.ProfileViewModel;
            }
            if(MyContactListViewModel == null)
            {
                EngineService.EngineInstance.ContactListViewModel = new ContactListViewModel();
                MyContactListViewModel = EngineService.EngineInstance.ContactListViewModel;
            }

            MyProfileViewModel.ME = MyApplication.Me;
            MyProfileViewModel.PropertyChanged += ProfileViewModel_PropertyChanged;

            MyContactListViewModel.PropertyChanged += ContactListViewModel_PropertyChanged;
            MyContactListViewModel.Items.CollectionChanged += ContactListViewModel_CollectionChanged;
            if (MyContactListViewModel.Items == null || MyContactListViewModel.Items.Count == 0)
            {
                MyContactListViewModel.LoadAllContactListItemCommand.Execute(MyApplication.Me.TOKEN);
            }
            InitViews();
		}

		private void ContactListViewModel_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                InitViews();
            });
        }

        protected override void OnStop()
		{
            base.OnStop();
            MyProfileViewModel.PropertyChanged -= ProfileViewModel_PropertyChanged;
            MyContactListViewModel.PropertyChanged -= ContactListViewModel_PropertyChanged;
		}

        private void ContactListViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(string.Equals(e.PropertyName, nameof(ContactListViewModel.IsBusy)))
            {
                if (MyContactListViewModel.IsBusy) { ShowLoadingDialog("Initialzing"); }
                else { HideLoadingDialog(); }
            }
        }

        private void ProfileViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(string.Equals(e.PropertyName, nameof(ProfileViewModel.IsBusy)))
            {
                if(MyProfileViewModel.IsBusy)
                {
                    ShowLoadingDialog(@"Update Information");
                }
                else
                {
                    HideLoadingDialog();
                }
            }

            if (string.Equals(e.PropertyName, nameof(ProfileViewModel.ME)) && MyProfileViewModel.ME != null)
            {
                MyApplication.Me = MyProfileViewModel.ME;
                InitViews();
            }
                
        }

        void InitViews()
        {
            //todo multi lang should be here
            var ActivityTitles = MyApplication.CurrentLangSetting.GetStringResourceContents();
            var showProfileTitles = MyApplication.CurrentLangSetting.GetShareProfilePhoto_Resource();
            var showStatusTitles = MyApplication.CurrentLangSetting.GetShareStatus_Resource();

            SupportActionBar.Title = ActivityTitles.GetValueOrDefault("privacy");
            txtWhoCanSee.Text = ActivityTitles.GetValueOrDefault("whocansee");
            txtProfilePhotoIndicator.Text = ActivityTitles.GetValueOrDefault("profile_photo");
            txtStatusIndicator.Text = ActivityTitles.GetValueOrDefault("status");
            txtMessaging.Text = ActivityTitles.GetValueOrDefault("messaging");

            if(MyContactListViewModel.Items == null || MyContactListViewModel.Items.Count == 0)
            {
                txtBlockedContactIndicator.Text = string.Format("{0} : 0", ActivityTitles.GetValueOrDefault("blocked_contact"));    
            }
            else
            {
                txtBlockedContactIndicator.Text = string.Format("{0} : {1}", ActivityTitles.GetValueOrDefault("blocked_contact"), MyContactListViewModel.Items.Where<ContactDTO>(u => u.IS_I_BLOCKED == true).ToList().Count);    
            }

            txtProfilePhoto.Text = showProfileTitles[MyProfileViewModel.ME.SHOW_PROFILE_TO];
            txtStatus.Text = showStatusTitles[MyProfileViewModel.ME.SHOW_STATUS_TO];
        }

		#region Button Click Handlers
		void BtProfilePhoto_Click(object sender, EventArgs e)
        {
            //show profile photo share bottom sheet dialog
            mBottomSheetDialog_ProfilePhoto = new BottomSheetDialog(this);
            View sheetView = LayoutInflater.Inflate(Resource.Layout.fragment_privacy_profile_photo, null);
            mBottomSheetDialog_ProfilePhoto.SetContentView(sheetView);
            InitProfilePhotoDialog(sheetView);
            mBottomSheetDialog_ProfilePhoto.Show();
        }

        void InitProfilePhotoDialog(View rootView)
        {
            TextView txtProfilePhotoIndicator1 = rootView.FindViewById<TextView>(Resource.Id.txtProfilePhotoIndicator);
            AppCompatRadioButton radioEveryOne = rootView.FindViewById<AppCompatRadioButton>(Resource.Id.radioEveryOne);
            AppCompatRadioButton radioMyContacts = rootView.FindViewById<AppCompatRadioButton>(Resource.Id.radioMyContacts);
            AppCompatRadioButton radioNobody = rootView.FindViewById<AppCompatRadioButton>(Resource.Id.radioNobody);
            radioEveryOne.Tag = 0;
            radioMyContacts.Tag = 1;
            radioNobody.Tag = 2;

            radioEveryOne.Click += ProfilePhoto_RadioChecked;
            radioMyContacts.Click += ProfilePhoto_RadioChecked;
            radioNobody.Click += ProfilePhoto_RadioChecked;

            var ActivityTitles = MyApplication.CurrentLangSetting.GetStringResourceContents();
            var showProfileTitles = MyApplication.CurrentLangSetting.GetShareProfilePhoto_Resource();

            txtProfilePhotoIndicator1.Text = ActivityTitles.GetValueOrDefault("profile_photo");
            radioEveryOne.Text = showProfileTitles[0];
            radioMyContacts.Text = showProfileTitles[1];
            radioNobody.Text = showProfileTitles[2];

            switch(MyProfileViewModel.ME.SHOW_PROFILE_TO)
            {
                case 0:
                    radioEveryOne.Checked = true;
                    break;
                case 1:
                    radioMyContacts.Checked = true;
                    break;
                case 2:
                    radioNobody.Checked = true;
                    break;
            }
        }

        private void ProfilePhoto_RadioChecked(object sender, EventArgs e)
        {
            mBottomSheetDialog_ProfilePhoto.Dismiss();
            var requestModel = new GetProfileRequest() { TOKEN = MyApplication.Me.TOKEN, USERID = Convert.ToString((sender as AppCompatRadioButton).Tag) };
            MyProfileViewModel.CommandUpdateProfileShareTo.Execute(requestModel);
        }

        void BtStatus_Click(object sender, EventArgs e)
        {
            //show status share bottom sheet dialog
            mBottomSheetDialog_StatusPhoto = new BottomSheetDialog(this);
            View sheetView = LayoutInflater.Inflate(Resource.Layout.fragment_privacy_status_share, null);
            mBottomSheetDialog_StatusPhoto.SetContentView(sheetView);
            InitStatusPhotoDialog(sheetView);
            mBottomSheetDialog_StatusPhoto.Show();
        }

        void InitStatusPhotoDialog(View rootView)
        {
            TextView StatusIndicator1 = rootView.FindViewById<TextView>(Resource.Id.txtStatusIndicator);
            TextView txtStatusExplain = rootView.FindViewById<TextView>(Resource.Id.txtStatusExplain);
            AppCompatRadioButton radioEveryOne = rootView.FindViewById<AppCompatRadioButton>(Resource.Id.radioEveryOne);
            AppCompatRadioButton radioMyContacts = rootView.FindViewById<AppCompatRadioButton>(Resource.Id.radioMyContacts);
            radioEveryOne.Tag = 0;
            radioMyContacts.Tag = 1;
            var ActivityTitles = MyApplication.CurrentLangSetting.GetStringResourceContents();
            var showStatusUpdate = MyApplication.CurrentLangSetting.GetShareStatus_Resource();
            StatusIndicator1.Text = ActivityTitles.GetValueOrDefault("whostatusupdate");
            txtStatusExplain.Text = ActivityTitles.GetValueOrDefault("statusexplain");
            radioEveryOne.Text = showStatusUpdate[0];
            radioMyContacts.Text = showStatusUpdate[1];
            switch (MyProfileViewModel.ME.SHOW_STATUS_TO)
            {
                case 0:
                    radioEveryOne.Checked = true;
                    break;
                case 1:
                    radioMyContacts.Checked = true;
                    break;
            }

            radioEveryOne.Click += StatusShare_RadioChanged;
            radioMyContacts.Click += StatusShare_RadioChanged;
        }

        private void StatusShare_RadioChanged(object sender, EventArgs e)
        {
            mBottomSheetDialog_StatusPhoto.Dismiss();
            var requestModel = new GetProfileRequest() { TOKEN = MyApplication.Me.TOKEN, USERID = Convert.ToString((sender as AppCompatRadioButton).Tag) };
            MyProfileViewModel.CommandUpdateStatusShareTo.Execute(requestModel);
        }

        void BtBlocked_Click(object sender, EventArgs e)
        {
            var mIntent = new Intent(this, typeof(PrivacyBlockedContactActivity));
            StartActivity(mIntent);
        }
        #endregion

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if(item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
            }
            return true;
        }
    }
}
