
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
using Android.Views;
using Android.Widget;
using API.Models.RequestModels;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Droid.Engine;
using WhatMessenger.Droid.GroupDetail.Adapters;
using WhatMessenger.Model;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.GroupDetail
{
    [Activity(Label = "")]
    public class ActivityEditGroupMember : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_edit_group_contact;
        Android.Support.V7.Widget.Toolbar toolbar;
        GroupListViewModel GroupViewModel;
        FloatingActionButton fabNext;
        RecyclerView recyclerSelected, recyclerContacts;
        //List<ChoosableContact> contactList, selectedList;
        ChooseContactAdapter chooseContactAdapter;
        SelectedContactAdapter selectedContactAdapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            GroupViewModel = EngineService.EngineInstance.GroupListViewModel;
            base.OnCreate(savedInstanceState);

            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            toolbar.Title = "";

            fabNext = FindViewById<FloatingActionButton>(Resource.Id.fabNext);
            fabNext.Click += FabNext_Click;

            recyclerContacts = FindViewById<RecyclerView>(Resource.Id.recyclerContacts);
            recyclerSelected = FindViewById<RecyclerView>(Resource.Id.horizontalContainer);

            LinearLayoutManager layoutManager = new LinearLayoutManager(this);
            recyclerContacts.SetLayoutManager(layoutManager);
            recyclerContacts.SetItemAnimator(new DefaultItemAnimator());
            chooseContactAdapter = new ChooseContactAdapter(this, GroupViewModel);
            recyclerContacts.SetAdapter(chooseContactAdapter);
            LinearLayoutManager horizontalManager = new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false);
            recyclerSelected.SetLayoutManager(horizontalManager);
            recyclerSelected.SetItemAnimator(new DefaultItemAnimator());
            selectedContactAdapter = new SelectedContactAdapter(this, GroupViewModel);
            recyclerSelected.SetAdapter(selectedContactAdapter);

        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
            }
            return true;
        }

        private void FabNext_Click(object sender, EventArgs e)
        {
            GroupViewModel.ChangeCurrentGroupMemberCommand.Execute(new GetProfileRequest(){ TOKEN = MyApplication.Me.TOKEN, USERID = $"{MyApplication.Me.USERID}"});
        }

        private void Adapter_ItemClick(object sender, RecyclerClickEventArgs e)
        {
            GroupViewModel.ChooseContactItemCommand.Execute(GroupViewModel.ChoosableContactList[e.Position]);
        }

        #region ViewModel Callback and Item Click Handlers
        protected override void OnStart()
        {
            base.OnStart();
            GroupViewModel.PropertyChanged += ViewModel_Property_Changed;
            chooseContactAdapter.ItemClick += Adapter_ItemClick;
            selectedContactAdapter.ItemClick += SelectedContactAdapter_ItemClick;
            if (GroupViewModel.ChoosableContactList.Count == 0 && MyApplication.Me != null)
            {
                var token = MyApplication.Me.TOKEN;
                GroupViewModel.LoadAllContactListCommand.Execute(token);
            }

            if(GroupViewModel.ChoosableContactList.Count() > 0)
            {
                UpdateChoosedContactList();
            }
        }

        protected override void OnStop()
        {
            base.OnStop();
            GroupViewModel.PropertyChanged -= ViewModel_Property_Changed;
            chooseContactAdapter.ItemClick -= Adapter_ItemClick;
            selectedContactAdapter.ItemClick -= SelectedContactAdapter_ItemClick;
        }

        void ViewModel_Property_Changed(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, nameof(GroupListViewModel.IsBusy)))
            {
                if (GroupViewModel.IsBusy)
                {
                    ShowLoadingDialog(@"Loading");
                }
                else
                {
                    HideLoadingDialog();
                    if(GroupViewModel.ChoosableContactList.Count() > 0)
                    {
                        UpdateChoosedContactList();
                    }
                }
            }

            if(string.Equals(e.PropertyName, nameof(GroupListViewModel.CurrentlyOpenDTO)))
            {
                Finish();
            }
        }

        void UpdateChoosedContactList()
        {
            GroupViewModel.ChoosedContactWithCurrentGroupDTOCommand.Execute(null);
        }

        void SelectedContactAdapter_ItemClick(object sender, Droid.RecyclerClickEventArgs e)
        {
            GroupViewModel.RemoveSelectedContactItemCommand.Execute(GroupViewModel.ChoosedContactList[e.Position]);
        }

        #endregion
    }
}
