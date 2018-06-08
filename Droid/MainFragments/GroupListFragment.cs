
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
using WhatMessenger.Droid.ChatDetailView;
using WhatMessenger.Droid.Engine;
using WhatMessenger.Droid.Fragments.Adapters;
using WhatMessenger.Droid.GroupDetail;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.Fragments
{
    public class GroupListFragment : MainBaseFragment
    {
        public static GroupListFragment GetInstance() => new GroupListFragment { Arguments = new Bundle() };

        RecyclerView recycler;
        FloatingActionButton fabAdd;
        SwipeRefreshLayout swipeRefresh;
        GroupListViewModel GroupListViewModel;
        GroupListAdapter Adapter;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            GroupListViewModel = EngineService.EngineInstance.GroupListViewModel;
            var rootView = inflater.Inflate(Resource.Layout.fragment_group, container, false);

            recycler = rootView.FindViewById<RecyclerView>(Resource.Id.recycleGroupHistory);
            fabAdd = rootView.FindViewById<FloatingActionButton>(Resource.Id.fabAdd);
            fabAdd.Click += FabAdd_Click;
            swipeRefresh = rootView.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefresh);
            InitSwipeRefreshLayout(swipeRefresh);

            recycler.HasFixedSize = true;
            recycler.SetLayoutManager(new GridLayoutManager(this.Context, 3));
            recycler.SetItemAnimator(new DefaultItemAnimator());

            Adapter = new GroupListAdapter(Activity, GroupListViewModel);
            recycler.SetAdapter(Adapter);

            return rootView;
        }

        void FabAdd_Click(object sender, EventArgs e)
        {
            var AddGroupIntent = new Intent(ParentActivity, typeof(AddGroupActivity));
            StartActivity(AddGroupIntent);
        }

        void Adapter_ItemClick(object sender, Droid.RecyclerClickEventArgs e)
        {
            var ClickedItem = GroupListViewModel.Items[e.Position];
            GroupListViewModel.CurrentlyOpenDTO = ClickedItem;

            var GroupDetailIntent = new Intent(ParentActivity, typeof(GroupChatDetailView));
            StartActivity(GroupDetailIntent);
        }

        void Refresher_Refresh(object sender, EventArgs e)
        {
            var token = ParentActivity.MyApplication.Me.TOKEN;
            GroupListViewModel.LoadAllGroupListCommand.Execute(token);
        }

        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(GroupListViewModel.IsBusy):
                    Activity.RunOnUiThread(() =>
                    {
                        if (GroupListViewModel.IsBusy && !swipeRefresh.Refreshing)
                        {
                            swipeRefresh.Refreshing = true;
                        }
                        else if (!GroupListViewModel.IsBusy)
                        {
                            swipeRefresh.Refreshing = false;
                        }
                    });
                    break;
            }
        }

        public override void OnStart()
        {
            base.OnStart();
            GroupListViewModel.AlreadyConnected = false;
            swipeRefresh.Refresh += Refresher_Refresh;
            GroupListViewModel.PropertyChanged += ViewModel_PropertyChanged;
            GroupListViewModel.GroupChatHistoryItemList.Clear();
            Adapter.ItemClick += Adapter_ItemClick;
            var userID = ParentActivity.MyApplication.Me.USERID;
            GroupListViewModel.GroupSocketStartCommand.Execute(userID);
            if (GroupListViewModel.Items.Count == 0)
            {
                var token = ParentActivity.MyApplication.Me.TOKEN;
                GroupListViewModel.LoadAllGroupListCommand.Execute(token);
            }
        }

        public override void OnStop()
        {
            base.OnStop();
            swipeRefresh.Refresh -= Refresher_Refresh;
            GroupListViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            Adapter.ItemClick -= Adapter_ItemClick;
        }
    }
}
