using System;
using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using WhatMessenger.Droid.Bases;

namespace WhatMessenger.Droid.CallHistory
{
    [Activity(Label = "")]
    public class CallHistoryDetailActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_call_info;
        RecyclerView recyclerCallHistory;
        ImageView imgProfile;
        TextView txtUserName;
        ImageButton btCall;
        Android.Support.V7.Widget.Toolbar toolbar;
        CallHistoryAdapter RecyclerAdapter;

        protected override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            imgProfile = FindViewById<ImageView>(Resource.Id.imgProfile);
            txtUserName = FindViewById<TextView>(Resource.Id.txtUserName);
            btCall = FindViewById<ImageButton>(Resource.Id.btCall);
            recyclerCallHistory = FindViewById<RecyclerView>(Resource.Id.recycleHistory);

            RecyclerAdapter = new CallHistoryAdapter(this);
            LinearLayoutManager layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            recyclerCallHistory.SetLayoutManager(layoutManager);
            recyclerCallHistory.SetAdapter(RecyclerAdapter);
        }

        public override bool OnCreateOptionsMenu(Android.Views.IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_call_detail, menu);
            return true;
        }

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
