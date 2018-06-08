using System;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Newtonsoft.Json;
using Plugin.Connectivity;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Droid.ContactDetail;
using WhatMessenger.Droid.Engine;
using WhatMessenger.Droid.Fragments.Adapters;
using WhatMessenger.ViewModel;
namespace WhatMessenger.Droid.Fragments
{
    public class ContactListFragment : MainBaseFragment
    {
        public static ContactListFragment GetInstance() => new ContactListFragment { Arguments = new Bundle() };
        RecyclerView recycler;
        FloatingActionButton fabAdd;
        SwipeRefreshLayout swipeRefresh;
        ContactListViewModel ContactListViewModel;
        ContactListAdapter Adapter;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if(EngineService.EngineInstance != null)
                ContactListViewModel = EngineService.EngineInstance.ContactListViewModel;
            
            var rootView = inflater.Inflate(Resource.Layout.fragment_contact, container, false);
            recycler = rootView.FindViewById<RecyclerView>(Resource.Id.recycleContactHistory);
            fabAdd = rootView.FindViewById<FloatingActionButton>(Resource.Id.btAddContact);
            swipeRefresh = rootView.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefresh);
            InitSwipeRefreshLayout(swipeRefresh);

            recycler.HasFixedSize = true;
            recycler.SetLayoutManager(new GridLayoutManager(this.Context, 3));
            recycler.SetItemAnimator(new DefaultItemAnimator());

            Adapter = new ContactListAdapter(Activity, ContactListViewModel);
            recycler.SetAdapter(Adapter);

            fabAdd.Click += FabAdd_Click;
            return rootView;
        }

        void FabAdd_Click(object sender, EventArgs e)
        {
            var IntentAdd = new Intent(this.Activity, typeof(AddContactActivity));
            StartActivity(IntentAdd);
        }

        void Adapter_ItemClick(object sender, Droid.RecyclerClickEventArgs e)
        {
            var IntentDetail = new Intent(this.Activity, typeof(ContactDetailViewActivity));
            IntentDetail.PutExtra(ContactDetailViewActivity.DETAIL_VIEW_USER_ID,ContactListViewModel.Items[e.Position].USERID);
            StartActivity(IntentDetail);
        }

        void Refresher_Refresh(object sender, EventArgs e)
        {
            if(CrossConnectivity.Current.IsConnected)
                ContactListViewModel.LoadAllContactListItemCommand.Execute(ParentActivity.MyApplication.Me.TOKEN);
        }

        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ContactListViewModel.IsBusy):
                    Activity.RunOnUiThread(() =>
                    {
                        if (ContactListViewModel.IsBusy && !swipeRefresh.Refreshing)
                        {
                            swipeRefresh.Refreshing = true;
                        }
                        else if (!ContactListViewModel.IsBusy)
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
            swipeRefresh.Refresh += Refresher_Refresh;
            ContactListViewModel.PropertyChanged += ViewModel_PropertyChanged;
            Adapter.ItemClick += Adapter_ItemClick;
            if (ContactListViewModel.Items.Count() == 0 && CrossConnectivity.Current.IsConnected)
            {
                ContactListViewModel.LoadAllContactListItemCommand.Execute(ParentActivity.MyApplication.Me.TOKEN);
            }
        }

        public override void OnStop()
        {
            base.OnStop();
            swipeRefresh.Refresh -= Refresher_Refresh;
            ContactListViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            Adapter.ItemClick -= Adapter_ItemClick;
        }
    }
}
