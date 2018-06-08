
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using WhatMessenger.Droid.Bases;
using WhatMessenger.Droid.Engine;
using WhatMessenger.Droid.Fragments.Adapters;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.Fragments
{
    public class StatusFragment : MainBaseFragment
    {
        public static StatusFragment GetInstance() => new StatusFragment { Arguments = new Bundle() };
        TextView txtCurrentIndicator, txtCurrentStatus, txtAboutIndicator;
        RecyclerView recycleStatusKinds;
        ImageButton btStatusEdit;
        StatusAdapter StatusAdapter;
        ProfileViewModel ThisProfileViewModel;
        List<string> TitleArray;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ThisProfileViewModel = EngineService.EngineInstance.ProfileViewModel;
            TitleArray = ParentActivity.MyApplication.CurrentLangSetting.GetStatusTitle_Resource();
            var rootView = inflater.Inflate(Resource.Layout.fragment_status, container, false);
            txtCurrentIndicator = rootView.FindViewById<TextView>(Resource.Id.txtCurrentIndicator);
            txtCurrentStatus = rootView.FindViewById<TextView>(Resource.Id.txtCurrentStatus);
            txtAboutIndicator = rootView.FindViewById<TextView>(Resource.Id.txtAboutIndicator);

            btStatusEdit = rootView.FindViewById<ImageButton>(Resource.Id.btStatusEdit);
            btStatusEdit.Click += StatusEdit_Click; 
            recycleStatusKinds = rootView.FindViewById<RecyclerView>(Resource.Id.recycleStatusKinds);
            StatusAdapter = new StatusAdapter(ParentActivity, ThisProfileViewModel, TitleArray);
            StatusAdapter.ItemClick += StatusAdapter_ItemClick;
            recycleStatusKinds.SetAdapter(StatusAdapter);
            recycleStatusKinds.SetLayoutManager(new LinearLayoutManager(this.ParentActivity));
            return rootView;
        }

        private void StatusEdit_Click(object sender, EventArgs e)
        {
            var mIntent = new Intent(ParentActivity, typeof(Status.ActivityStatusEdit));
            StartActivity(mIntent);
        }

        void StatusAdapter_ItemClick(object sender, RecyclerClickEventArgs e)
        {
            if (Plugin.Connectivity.CrossConnectivity.Current.IsConnected)
            {
                string TitleValue = TitleArray[e.Position];
                var request = new GetProfileRequest() { TOKEN = ParentActivity.MyApplication.Me.TOKEN, USERID = Convert.ToString(TitleValue) };
                ThisProfileViewModel.CommandUpdateStatusTitle.Execute(request);
            }
            else
            {
                DialogUtils.ShowOKDialog(ParentActivity, @"Warning", @"No Internet Connection");
            }
        }


        public override void OnStart()
        {
            base.OnStart();
            if(ThisProfileViewModel == null)
            {
                EngineService.EngineInstance.ProfileViewModel = new ProfileViewModel();
                ThisProfileViewModel = EngineService.EngineInstance.ProfileViewModel;
            }
            if (ThisProfileViewModel.ME == null) ThisProfileViewModel.ME = ParentActivity.MyApplication.Me;
            ThisProfileViewModel.PropertyChanged += ProfileViewModel_PropertyChanged;
            if(string.IsNullOrEmpty(ThisProfileViewModel.ME.STATUS_INDICATOR))
            {
                ThisProfileViewModel.ME.STATUS_INDICATOR = "Hi, there, I am using NightOwl";
                ParentActivity.MyApplication.Me.STATUS_INDICATOR = "Hi, there, I am usnig NightOwl";
            }
            txtCurrentStatus.Text = ThisProfileViewModel.ME.STATUS_INDICATOR.Trim();
        }

        private void ProfileViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(string.Equals(e.PropertyName, nameof(ProfileViewModel.IsBusy)))
            {
                ShowLoadingDialog("Update Status");
            }
            else
            {
                HideLoadingDialog();
            }
            if(string.Equals(e.PropertyName, nameof(ProfileViewModel.ME)))
            {
                ParentActivity.MyApplication.Me = ThisProfileViewModel.ME;
                txtCurrentStatus.Text = ThisProfileViewModel.ME.STATUS_INDICATOR;
            }
        }

        public override void OnStop()
        {
            base.OnStop();
            ThisProfileViewModel.PropertyChanged -= ProfileViewModel_PropertyChanged;
        }
    }
}
