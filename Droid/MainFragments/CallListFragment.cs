
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Droid.CallHistory;
using WhatMessenger.Droid.Fragments.Adapters;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.Fragments
{
    public class CallListFragment : MainBaseFragment
    {
        public static CallListFragment GetInstance() => new CallListFragment { Arguments = new Bundle() };

        RecyclerView recycler;
        FloatingActionButton fabAddCall;
        SwipeRefreshLayout swipeRefresh;
        CallListViewModel CallListViewModel;
        CallListAdapter Adapter;


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            CallListViewModel = new CallListViewModel();


            var rootView = inflater.Inflate(Resource.Layout.fragment_call, container, false);

            recycler = rootView.FindViewById<RecyclerView>(Resource.Id.recycleCallHistory);
            fabAddCall = rootView.FindViewById<FloatingActionButton>(Resource.Id.fabCall);
            swipeRefresh = rootView.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefresh);
            InitSwipeRefreshLayout(swipeRefresh);

            recycler.HasFixedSize = true;
            recycler.SetLayoutManager(new GridLayoutManager(this.Context, 2));
            recycler.SetItemAnimator(new DefaultItemAnimator());

            Adapter = new CallListAdapter(Activity, CallListViewModel);
            recycler.SetAdapter(Adapter);

            InitRecyclerScrollListener(recycler, (e) => {
                if (e && fabAddCall.IsShown)
                {
                    fabAddCall.Hide();
                }
                else if (!e && !fabAddCall.IsShown)
                {
                    fabAddCall.Show();
                }
            });

            return rootView;
        }

        void SwipeRefresherHandle(object sender, EventArgs e)
        {
            
            CallListViewModel.LoadAllCallListItem.Execute(null);
        }

        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(CallListViewModel.IsBusy):
                    Activity.RunOnUiThread(() =>
                    {
                        if (CallListViewModel.IsBusy && !swipeRefresh.Refreshing)
                        {
                            swipeRefresh.Refreshing = true;
                        }
                        else if (!CallListViewModel.IsBusy)
                        {
                            swipeRefresh.Refreshing = false;
                            //adapter.NotifyDataSetChanged();
                        }
                    });
                    break;
            }
        }

        public override void OnStart()
        {
            base.OnStart();
            swipeRefresh.Refresh += SwipeRefresherHandle;
            Adapter.ItemClick += Adapter_ItemClick;
            Adapter.ItemLongClick += Adapter_ItemLongClick;
            CallListViewModel.PropertyChanged += ViewModel_PropertyChanged;

            if (CallListViewModel.Items.Count() == 0)
            {
                CallListViewModel.LoadAllCallListItem.Execute(null);
            }
        }

        private void Adapter_ItemLongClick(object sender, RecyclerClickEventArgs e)
        {
            
        }


        private void Adapter_ItemClick(object sender, RecyclerClickEventArgs e)
        {
            var CallIntent = new Intent(Activity, typeof(CallHistoryDetailActivity));
            StartActivity(CallIntent);
        }

        public override void OnStop()
        {
            base.OnStop();
            swipeRefresh.Refresh -= SwipeRefresherHandle;
            CallListViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            Adapter.ItemClick -= Adapter_ItemClick;
            Adapter.ItemLongClick -= Adapter_ItemLongClick;
        }

    }
}
