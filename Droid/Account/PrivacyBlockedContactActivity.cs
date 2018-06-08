
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
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
using WhatMessenger.Droid.Bases;
using WhatMessenger.Droid.Engine;
using WhatMessenger.Model.BaseModel;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.Account
{
    [Activity(Label = "PrivacyBlockedContactActivity")]
    public class PrivacyBlockedContactActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_privacy_blocked_contact;
        RecyclerView recyclerContent;
        LinearLayout layout_no_blocked;
        Adapters.BlockedContactListAdapter Adapter;
        ContactListViewModel ThisContactViewModel;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            ThisContactViewModel = EngineService.EngineInstance.ContactListViewModel;
            base.OnCreate(savedInstanceState);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Title = "Blocked Contacts";
            recyclerContent = FindViewById<RecyclerView>(Resource.Id.recyclerContent);
            layout_no_blocked = FindViewById<LinearLayout>(Resource.Id.layout_no_blocked);

            Adapter = new Adapters.BlockedContactListAdapter(this, ThisContactViewModel);

            Adapter.ItemClick += Adapter_ContactItemClick;

            GridLayoutManager layoutManager = new GridLayoutManager(this, 3);
            recyclerContent.SetLayoutManager(layoutManager);
            recyclerContent.SetAdapter(Adapter);
        }

        void Adapter_ContactItemClick(object sender, RecyclerClickEventArgs e)
        {
            new Android.Support.V7.App.AlertDialog.Builder(this)
                       .SetTitle("Unblock Contact")
                       .SetMessage("Will you unblock Contact?")
                       .SetPositiveButton("Ok", (senderOk, eOk) =>
                       {
                            (senderOk as IDialogInterface).Dismiss();
                            var blockedList = ThisContactViewModel.Items.Where(u => u.IS_I_BLOCKED).ToList();
                            UnBlockAccount(Convert.ToString(blockedList[e.Position].CONTACT_ID));
                       })
                       .SetNegativeButton("Cancel", (senderCancel, eCancel) =>{
                            (senderCancel as IDialogInterface).Dismiss();
                        }).Show();
        }

        public void UnBlockAccount(string ContactID)
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                var model = new GetProfileRequest() { TOKEN = MyApplication.Me.TOKEN, USERID = ContactID };
                ThisContactViewModel.UnblockContactCommand.Execute(model);
            }
            else
            {
                DialogUtils.ShowOKDialog(this, @"Warning", @"No Internet Connection");
            }
        }

        protected override void OnStop()
        {
            base.OnStop();
            ThisContactViewModel.PropertyChanged -= ContactListViewModel_PropertyChanged;
            ThisContactViewModel.Items.CollectionChanged -= ContactListViewModel_CollectionChanged;
        }

        protected override void OnStart()
        {
            base.OnStart();
            if(ThisContactViewModel == null)
            {
                EngineService.EngineInstance.ContactListViewModel = new ContactListViewModel();
                ThisContactViewModel = EngineService.EngineInstance.ContactListViewModel;
            }

            ThisContactViewModel.PropertyChanged += ContactListViewModel_PropertyChanged;
            ThisContactViewModel.Items.CollectionChanged += ContactListViewModel_CollectionChanged;
            if (ThisContactViewModel.Items.Count() == 0 && CrossConnectivity.Current.IsConnected)
            {
                ThisContactViewModel.LoadAllContactListItemCommand.Execute(MyApplication.Me.TOKEN);
            }
            ShowHideIndicatorLayout();
        }

        void ShowHideIndicatorLayout()
        {
            RunOnUiThread(() =>
            {
                if (ThisContactViewModel.Items.Where<ContactDTO>(u => u.IS_I_BLOCKED).ToList().Count == 0)
                {
                    layout_no_blocked.Visibility = ViewStates.Visible;
                    recyclerContent.Visibility = ViewStates.Invisible;
                }
                else
                {
                    layout_no_blocked.Visibility = ViewStates.Invisible;
                    recyclerContent.Visibility = ViewStates.Visible;
                }
            });
        }
        private void ContactListViewModel_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ShowHideIndicatorLayout();
        }

        private void ContactListViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(string.Equals(e.PropertyName, nameof(ContactListViewModel.IsBusy)))
            {
                if(ThisContactViewModel.IsBusy)
                {
                    ShowLoadingDialog("Initializing");
                }
                else
                {
                    HideLoadingDialog();
                }
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_add_block, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if(item.ItemId == Resource.Id.menuAdd)
            {
                var mIntent = new Intent(this, typeof(PrivacyAddBlockContactActivity));
                StartActivity(mIntent);
            }
            else if(item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
            }
            return true;
        }
    }
}