
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
using WhatMessenger.Droid.Bases;
using WhatMessenger.Droid.Engine;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.GroupDetail
{
    [Activity(Label = "")]
    public class AddGroupActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_add_group;
        TextView txtPrimaryToolbar, txtSecondaryToolbar;
        Android.Support.V7.Widget.Toolbar toolbar;
        private int CURRENT_FRAGMENT = 0;
        public const int FRAGMENT_CHOOSE_CONTACT = 0;
        public const int FragmentSetGroupName = 1;
        GroupListViewModel ViewModel;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            ViewModel = EngineService.EngineInstance.GroupListViewModel;
            ViewModel.ChoosableContactList.Clear();
            ViewModel.ChoosedContactList.Clear();

            base.OnCreate(savedInstanceState);
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            txtPrimaryToolbar = FindViewById<TextView>(Resource.Id.txtPrimaryToolbar);
            txtSecondaryToolbar = FindViewById<TextView>(Resource.Id.txtDescriptionToolbar);
            SetFragment(FRAGMENT_CHOOSE_CONTACT);
        }

		protected override void OnDestroy()
		{
           
            ViewModel.ChoosedContactList.Clear();
            ViewModel.ChoosedContactList.Clear();
            base.OnDestroy();
		}
		//public override bool OnCreateOptionsMenu(IMenu menu)
		//{
		//    MenuInflater.Inflate(Resource.Menu.menu_search, menu);
		//    return true;
		//}

		public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (CURRENT_FRAGMENT == FRAGMENT_CHOOSE_CONTACT)
            {
                if (item.ItemId == Android.Resource.Id.Home)
                {
                    Finish();
                }
            }
            else
            {
                SetFragment(CURRENT_FRAGMENT - 1);
            }
            return true;
        }

        #region toolbar management part
        public void SetPrimaryTextContent(string value)
        {
            RunOnUiThread(() =>
            {
                txtPrimaryToolbar.Text = value;
            });
        }

        public void SetSecondaryTextContent(string value)
        {
            RunOnUiThread(() => {
                txtSecondaryToolbar.Text = value;    
            });
        }

        public void SetSecondaryVisibility(bool value)
        {
            RunOnUiThread(() => {
                txtSecondaryToolbar.Visibility = value == true ? ViewStates.Visible : ViewStates.Gone;    
            });
        }
        #endregion

        #region Fragment Management part
        public void SetFragment(int TargetFragment)
        {
            Android.Support.V4.App.Fragment f = null;
            switch (TargetFragment)
            {
                case FRAGMENT_CHOOSE_CONTACT:
                    f = ChooseContactFragment.GetInstance();
                    break;
                case FragmentSetGroupName:
                    f = SetGroupNameFragment.GetInstance();
                    break;
            }
            CURRENT_FRAGMENT = TargetFragment;
            AttachFragment(f);
        }

        void AttachFragment(Android.Support.V4.App.Fragment f)
        {
            if (f != null)
            {
                while (SupportFragmentManager.BackStackEntryCount > 0)
                {
                    SupportFragmentManager.PopBackStackImmediate();
                }
                SupportFragmentManager.BeginTransaction()
                                      .Replace(Resource.Id.groupContainer, f)
                                      .Commit();
            }
        }

        #endregion
    }
}
