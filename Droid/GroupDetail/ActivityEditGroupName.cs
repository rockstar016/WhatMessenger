
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using API.Models.RequestModels;
using Rock.Utils;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Droid.Engine;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.GroupDetail
{
    [Activity(Label = "")]
    public class ActivityEditGroupName : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_edit_groupname;
        Android.Support.V7.Widget.Toolbar toolbar;
        TextView txtStatusIndicator;
        EditText txtGroupName;
        Button btCancel, btDone;
        GroupListViewModel GroupViewModel;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            GroupViewModel = EngineService.EngineInstance.GroupListViewModel;
            base.OnCreate(savedInstanceState);
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            toolbar.Title = "";
            txtStatusIndicator = FindViewById<TextView>(Resource.Id.txtStatusIndicator);
            txtGroupName = FindViewById<EditText>(Resource.Id.txtGroupName);
            btCancel = FindViewById<Button>(Resource.Id.btCancel);
            btDone = FindViewById<Button>(Resource.Id.btDone);
            btCancel.Click += (sender, e) => { Finish(); };
            btDone.Click += BtDone_Click;
        }

        void BtDone_Click(object sender, EventArgs e)
        {
            if (StringCheckUtil.isEmpty(txtGroupName)) return;
            GroupViewModel.ChangeCurrentGroupNameCommand.Execute(new ContactAddRequest() { TOKEN = MyApplication.Me.TOKEN, MY_ID = Convert.ToString(GroupViewModel.CurrentlyOpenDTO.GROUP_ID), OTHER_ID = txtGroupName.Text });
        }

		protected override void OnStart()
		{
            base.OnStart();
            InitView();
            GroupViewModel.PropertyChanged += GroupViewModel_PropertyChanged;
		}

        void InitView()
        {
            txtGroupName.Text = GroupViewModel.CurrentlyOpenDTO.GROUP_NAME;
        }

		protected override void OnStop()
		{
            base.OnStop();
            GroupViewModel.PropertyChanged -= GroupViewModel_PropertyChanged;
		}

        void GroupViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(string.Equals(e.PropertyName, nameof(GroupListViewModel.IsBusy)))
            {
                if(GroupViewModel.IsBusy)
                {
                    RunOnUiThread(() =>
                    {
                        ShowLoadingDialog("Update Name");
                    });
                }
                else
                {
                    RunOnUiThread(() =>
                    {
                        HideLoadingDialog();
                    });
                }
            }
            if(string.Equals(e.PropertyName, nameof(GroupListViewModel.CurrentlyOpenDTO)))
            {
                if(GroupViewModel != null)
                {
                    RunOnUiThread(() =>
                    {
                        Finish();
                    });
                }
            }
        }

		public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
            }
            return true;
        }
    }
}
