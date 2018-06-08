
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
using Org.Apache.Http.Authentication;
using WhatMessenger.Droid.Engine;
using WhatMessenger.Droid.GroupDetail.Adapters;
using WhatMessenger.Model;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.GroupDetail
{
    public class ChooseContactFragment : GroupBaseFragment
    {
        public static ChooseContactFragment GetInstance() => new ChooseContactFragment { Arguments = new Bundle() };

        FloatingActionButton fabNext;
        RecyclerView recyclerSelected, recyclerContacts;
        ChooseContactAdapter chooseContactAdapter;
        SelectedContactAdapter selectedContactAdapter;
        GroupListViewModel GroupListViewModel;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            GroupListViewModel = EngineService.EngineInstance.GroupListViewModel;
            var rootView = inflater.Inflate(Resource.Layout.fragment_group_selectcontact, container, false);
            fabNext = rootView.FindViewById<FloatingActionButton>(Resource.Id.fabNext);
            fabNext.Click += FabNext_Click;

            recyclerContacts = rootView.FindViewById<RecyclerView>(Resource.Id.recyclerContacts);
            recyclerSelected = rootView.FindViewById<RecyclerView>(Resource.Id.horizontalContainer);

            LinearLayoutManager layoutManager = new LinearLayoutManager(ParentActivity);
            recyclerContacts.SetLayoutManager(layoutManager);
            recyclerContacts.SetItemAnimator(new DefaultItemAnimator());
            chooseContactAdapter = new ChooseContactAdapter(ParentActivity, GroupListViewModel);
            recyclerContacts.SetAdapter(chooseContactAdapter);
            LinearLayoutManager horizontalManager = new LinearLayoutManager(ParentActivity, LinearLayoutManager.Horizontal, false);
            recyclerSelected.SetLayoutManager(horizontalManager);
            recyclerSelected.SetItemAnimator(new DefaultItemAnimator());
            selectedContactAdapter = new SelectedContactAdapter(this.ParentActivity, GroupListViewModel);
            recyclerSelected.SetAdapter(selectedContactAdapter);
            return rootView;
        }

        void FabNext_Click(object sender, EventArgs e)
        {
            ParentActivity.SetFragment(AddGroupActivity.FragmentSetGroupName);
        }

        private void Adapter_ItemClick(object sender, RecyclerClickEventArgs e)
        {
            GroupListViewModel.ChooseContactItemCommand.Execute(GroupListViewModel.ChoosableContactList[e.Position]);
        }

        #region ViewModel Observer Listener
        public override void OnStart()
        {
            base.OnStart();
            GroupListViewModel.PropertyChanged += ViewModel_Property_Changed;
            chooseContactAdapter.ItemClick += Adapter_ItemClick;
            selectedContactAdapter.ItemClick += SelectedContactAdapter_ItemClick;
            GroupListViewModel.ChoosedContactList.CollectionChanged += ChoosedContactList_CollectionChanged;
            if (GroupListViewModel.ChoosableContactList.Count == 0 && ParentActivity.MyApplication.Me != null)
            {
                var token = ParentActivity.MyApplication.Me.TOKEN;
                GroupListViewModel.LoadAllContactListCommand.Execute(token);
            }
            ChoosedContactList_CollectionChanged(null, null);
        }

        public override void OnStop()
        {
            base.OnStop();
            GroupListViewModel.PropertyChanged -= ViewModel_Property_Changed;
            GroupListViewModel.ChoosedContactList.CollectionChanged -= ChoosedContactList_CollectionChanged;
            chooseContactAdapter.ItemClick -= Adapter_ItemClick;
            selectedContactAdapter.ItemClick -= SelectedContactAdapter_ItemClick;
        }

        void SelectedContactAdapter_ItemClick(object sender, Droid.RecyclerClickEventArgs e)
        {
            GroupListViewModel.RemoveSelectedContactItemCommand.Execute(GroupListViewModel.ChoosedContactList[e.Position]);
        }

        void ChoosedContactList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ParentActivity.SetSecondaryTextContent(GroupListViewModel.ChoosedContactList.Count == 0 ? "Choose contact items" : $"{GroupListViewModel.ChoosedContactList.Count} of {GroupListViewModel.ChoosableContactList.Count} Selected");
        }

        void ViewModel_Property_Changed(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, nameof(GroupListViewModel.IsBusy)))
            {
                if (GroupListViewModel.IsBusy)
                {
                    ShowLoadingDialog(@"Loading");
                }
                else
                {
                    HideLoadingDialog();
                }
            }
        }
        #endregion
    }
}
