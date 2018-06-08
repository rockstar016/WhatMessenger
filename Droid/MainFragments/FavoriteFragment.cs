
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using WhatMessenger.Droid.Bases;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.Fragments
{
    public class FavoriteFragment : MainBaseFragment
    {
        public static FavoriteFragment GetInstance() => new FavoriteFragment { Arguments = new Bundle() };
        RecyclerView recycleFavorite;
        SwipeRefreshLayout swipeRefresh;
        LinearLayout fragStatus;
        FavoriteViewModel ViewModel;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return 
            ViewModel = new FavoriteViewModel();
            var rootView = inflater.Inflate(Resource.Layout.fragment_favorite, container, false);

            recycleFavorite = rootView.FindViewById<RecyclerView>(Resource.Id.recycleFavorite);

            swipeRefresh = rootView.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefresh);
            InitSwipeRefreshLayout(swipeRefresh);
            fragStatus =  rootView.FindViewById<LinearLayout>(Resource.Id.fragStatus);

            return rootView;
        }

        public override void OnStart()
        {
            base.OnStart();
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            swipeRefresh.Refresh += Refresher_Refresh;
            //Adapter.ItemClick += Adapter_ItemClick;
            if (ViewModel.Items.Count() == 0)
            {
                ViewModel.CommandLoadAllFavoriteListItem.Execute(null);
            }
        }

        public override void OnStop()
        {
            base.OnStop();
            swipeRefresh.Refresh -= Refresher_Refresh;
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            //Adapter.ItemClick -= Adapter_ItemClick;
        }

        void Refresher_Refresh(object sender, EventArgs e)
        {
            ViewModel.CommandLoadAllFavoriteListItem.Execute(null);
        }

        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ContactListViewModel.IsBusy):
                    Activity.RunOnUiThread(() =>
                    {
                        if (ViewModel.IsBusy && !swipeRefresh.Refreshing)
                        {
                            swipeRefresh.Refreshing = true;
                        }
                        else if (!ViewModel.IsBusy)
                        {
                            swipeRefresh.Refreshing = false;
                            //adapter.NotifyDataSetChanged();
                        }
                    });
                    break;
            }
        }

    }
}
