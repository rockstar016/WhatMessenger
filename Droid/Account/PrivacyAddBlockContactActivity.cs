
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using API.Models.RequestModels;
using Plugin.Connectivity;
using Rock.Utils;
using WhatMessenger.Droid.Account.Adapters;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Droid.Engine;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.Account
{
    [Activity(Label = "PrivacyAddBlockContactActivity")]
    public class PrivacyAddBlockContactActivity : BaseActivity
    {
        ContactListViewModel ThisContactListViewModel;
        protected override int LayoutResource => Resource.Layout.activity_add_contact_block;
        RecyclerView recyclerContent;
        AddContactToBlockAdapter mAdapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            ThisContactListViewModel = EngineService.EngineInstance.ContactListViewModel;
            base.OnCreate(savedInstanceState);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Title = "Add Contact to block";

            recyclerContent = FindViewById<RecyclerView>(Resource.Id.recyclerContent);
            LinearLayoutManager layoutManager = new LinearLayoutManager(this);
            mAdapter = new AddContactToBlockAdapter(this, ThisContactListViewModel);

            mAdapter.ItemClick += MAdapter_ContactItemClick;
            recyclerContent.SetAdapter(mAdapter);
            recyclerContent.SetLayoutManager(layoutManager);
        }

        void MAdapter_ContactItemClick(object sender, RecyclerClickEventArgs e)
        {
            //update contact list with block menu
            if(CrossConnectivity.Current.IsConnected)
            {
                var dataProvider = ThisContactListViewModel.Items.Where(u => u.IS_I_BLOCKED == false).ToList();
                var model = new GetProfileRequest() { TOKEN = MyApplication.Me.TOKEN, USERID = Convert.ToString(dataProvider[e.Position].CONTACT_ID) };
                ThisContactListViewModel.BlockContactCommand.Execute(model);
            }
            else
            {
                DialogUtils.ShowOKDialog(this, @"Warning", @"No Internet Connection");
            }
        }


		protected override void OnStart()
		{
            base.OnStart();
            if(ThisContactListViewModel == null)
            {
                EngineService.EngineInstance.ContactListViewModel = new ContactListViewModel();
                ThisContactListViewModel = EngineService.EngineInstance.ContactListViewModel;
            }
            ThisContactListViewModel.PropertyChanged += ThisContactListViewModel_PropertyChanged;
            if (ThisContactListViewModel.Items.Count() == 0 && CrossConnectivity.Current.IsConnected)
            {
                ThisContactListViewModel.LoadAllContactListItemCommand.Execute(MyApplication.Me.TOKEN);
            }
		}

        void ThisContactListViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(string.Equals(e.PropertyName, nameof(ContactListViewModel.IsBusy)))
            {
                ShowLoadingDialog(@"Blocking contact");
            }
            else
            {
                HideLoadingDialog();
            }
        }

		protected override void OnStop()
		{
            base.OnStop();
            ThisContactListViewModel.PropertyChanged -= ThisContactListViewModel_PropertyChanged;
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
