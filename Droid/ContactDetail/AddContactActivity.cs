
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using API.Models.RequestModels;
using Plugin.Connectivity;
using Rock.Utils;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Droid.ContactDetail.Adapter;
using WhatMessenger.Droid.Engine;
using WhatMessenger.Model.Constants;
using WhatMessenger.Model.RequestModels;

namespace WhatMessenger.Droid.ContactDetail
{
    [Activity(Label = "", Theme = "@style/AppTheme.NoActionBar")]
    public class AddContactActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_add_contact;
        Android.Support.V7.Widget.Toolbar toolbar;
        Android.Support.V7.Widget.SearchView searchView;
        RecyclerView recyclerView;
        ContentLoadingProgressBar progressMore;
        CandiateAdapter Adapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerContent);

            progressMore = FindViewById<ContentLoadingProgressBar>(Resource.Id.progressMore);
            progressMore.Visibility = ViewStates.Invisible;

            InitEngineService();
            InitViews();
        }

        protected override void OnStart()
        {
            base.OnStart();
            EngineService.EngineInstance.ContactListViewModel.PropertyChanged += ViewModel_PropertyChanged;
            Adapter.ItemClick += Adapter_ItemClick;
            Adapter.ItemLongClick += Adapter_ItemLongClick;
        }

        protected override void OnStop()
        {
            base.OnStop();
            EngineService.EngineInstance.ContactListViewModel.PropertyChanged += ViewModel_PropertyChanged;
            Adapter.ItemClick += Adapter_ItemClick;
            Adapter.ItemLongClick += Adapter_ItemLongClick;
        }
        private void Adapter_ItemLongClick(object sender, RecyclerClickEventArgs e)
        {
            
        }

        private void Adapter_ItemClick(object sender, RecyclerClickEventArgs e)
        {
            if(EngineService.EngineInstance.ContactListViewModel.CandiateList != null && CrossConnectivity.Current.IsConnected)
            {
                var otherModel = EngineService.EngineInstance.ContactListViewModel.CandiateList[e.Position];
                var AddReqeust = new ContactAddRequest() { TOKEN = MyApplication.Me.TOKEN, MY_ID = $"{MyApplication.Me.USERID}", OTHER_ID = $"{otherModel.USERID}" };
                EngineService.EngineInstance.ContactListViewModel.AddContactListItemCommand.Execute(AddReqeust);    
            }
            else
            {
                DialogUtils.ShowOKDialog(this, @"warning", @"No Internet Connection");
            }
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(EngineService.EngineInstance.ContactListViewModel.IsBusy):
                    RunOnUiThread(() =>
                    {
                        if (EngineService.EngineInstance.ContactListViewModel.IsBusy)
                        {
                            progressMore.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            progressMore.Visibility = ViewStates.Gone;
                        }
                    });
                    break;
                case nameof(EngineService.EngineInstance.ContactListViewModel.AddContactHappen):
                    RunOnUiThread(() =>
                    {
                        if (EngineService.EngineInstance.ContactListViewModel.AddContactHappen.Equals(GlobalConstants.RESPONSE_DUPLICATE_ERROR))
                        {
                            RunOnUiThread(() =>
                            {
                                DialogUtils.ShowOKDialog(this, @"warning", @"Contact is already exists");
                            });
                        }
                        else if(EngineService.EngineInstance.ContactListViewModel.AddContactHappen.Equals(GlobalConstants.TOKEN_VERIFY_ERROR))
                        {
                            RunOnUiThread(() =>
                            {
                                DialogUtils.ShowOKDialog(this, @"warning", @"Invalid authentication");
                            });
                        }
                        else if(string.IsNullOrEmpty(EngineService.EngineInstance.ContactListViewModel.AddContactHappen))
                        {
                            RunOnUiThread(()=>{
                                DialogUtils.ShowOKDialog(this, @"success", @"Contact request sent successfully.");    
                            });
                        }
                        else
                        {
                            RunOnUiThread(() =>
                            {
                                DialogUtils.ShowOKDialog(this, @"warning", @"Error is occured on server");
                            });
                        }

                    });
                    break;
            }
        }

        private void InitViews()
        {
            recyclerView.HasFixedSize = true;
            recyclerView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Vertical, false));
            recyclerView.SetItemAnimator(new DefaultItemAnimator());

            Adapter = new CandiateAdapter(this, EngineService.EngineInstance.ContactListViewModel);
            recyclerView.SetAdapter(Adapter);
        }

        async void InitEngineService()
        {
            if(EngineService.EngineInstance == null)
            {
                var EngineService = new Intent(this, typeof(EngineService));
                StartService(EngineService);
                await Task.Delay(100);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_search, menu);
            searchView = menu.FindItem(Resource.Id.menuSearch).ActionView as Android.Support.V7.Widget.SearchView;
            searchView.QueryTextSubmit += SearchView_QueryTextSubmit;
            searchView.QueryTextChange += SearchView_QueryTextChange;
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
            }
            return true;
        }

        void SearchView_QueryTextSubmit(object sender, Android.Support.V7.Widget.SearchView.QueryTextSubmitEventArgs e)
        {
            
            if (!string.IsNullOrEmpty(e.Query) && CrossConnectivity.Current.IsConnected)
            {
                ContactCandidateRequest model = new ContactCandidateRequest() { KEYWORD = e.Query, TOKEN = MyApplication.Me.TOKEN };
                EngineService.EngineInstance.ContactListViewModel.LoadAllCandiateListItemCommand.Execute(model);
            }
            else
            {
                DialogUtils.ShowOKDialog(this, @"warning", "Network is disconnected");
            }
        }

        void SearchView_QueryTextChange(object sender, Android.Support.V7.Widget.SearchView.QueryTextChangeEventArgs e)
        {
            if(string.IsNullOrEmpty(e.NewText))
            {
                EngineService.EngineInstance.ContactListViewModel.ClearCandidateListItemCommand.Execute(null);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            EngineService.EngineInstance.ContactListViewModel.ClearCandidateListItemCommand.Execute(null);
        }
    }
}
