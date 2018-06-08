
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
using API.Models.RequestModels;
using Rock.Utils;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Droid.Engine;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.Status
{
    [Activity(Label = "ActivityStatusEdit")]
    public class ActivityStatusEdit : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_edit_status;
        TextView txtStatusIndicator;
        EditText txtStatusContent;
        Button btCancel, btDone;
        ProfileViewModel ThisProfileViewModel;

        Android.Support.V7.Widget.Toolbar toolbar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            ThisProfileViewModel = EngineService.EngineInstance.ProfileViewModel;

            base.OnCreate(savedInstanceState);
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            btCancel = FindViewById<Button>(Resource.Id.btCancel);
            btDone = FindViewById<Button>(Resource.Id.btDone);
            txtStatusContent = FindViewById<EditText>(Resource.Id.txtStatusContent);
            txtStatusIndicator = FindViewById<TextView>(Resource.Id.txtStatusIndicator);
            btCancel.Click += OnClick_CancelListener;
            btDone.Click += OnClick_DoneListener;
        }

        void InitViews()
        {
            txtStatusContent.Text = ThisProfileViewModel.ME.STATUS_INDICATOR;
        }

        private void OnClick_DoneListener(object sender, EventArgs e)
        {
            if (StringCheckUtil.isEmpty(txtStatusContent)) return;
                
            if (Plugin.Connectivity.CrossConnectivity.Current.IsConnected)
            {
                string TitleValue = txtStatusContent.Text.Trim();
                var request = new GetProfileRequest() { TOKEN = MyApplication.Me.TOKEN, USERID = Convert.ToString(TitleValue) };
                ThisProfileViewModel.CommandUpdateStatusTitle.Execute(request);
            }
            else
            {
                DialogUtils.ShowOKDialog(this, @"Warning", @"No Internet Connection");
            }
        }

        private void OnClick_CancelListener(object sender, EventArgs e)
        {
            Finish();
        }

		protected override void OnStart()
		{
            base.OnStart();
            if(ThisProfileViewModel == null)
            {
                EngineService.EngineInstance.ProfileViewModel = new ProfileViewModel();
                ThisProfileViewModel = EngineService.EngineInstance.ProfileViewModel;
            }
            ThisProfileViewModel.PropertyChanged += ThisProfileViewModel_PropertyChanged;
            InitViews();
		}

        void ThisProfileViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(string.Equals(e.PropertyName, nameof(ProfileViewModel.IsBusy)))
            {
                ShowLoadingDialog("Update Status");
            }
            else
            {
                HideLoadingDialog();
                Finish();
            }
        }

		protected override void OnStop()
		{
            base.OnStop();
            ThisProfileViewModel.PropertyChanged -= ThisProfileViewModel_PropertyChanged;
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
            }
            return true;
        }
    }
}
