using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.Design.Widget;
using WhatMessenger.Droid.Bases;
using static Android.Support.Design.Widget.TabLayout;
using WhatMessenger.Droid.BaseUI;
using WhatMessenger.Droid.Fragments;
using Rock.Utils;
using TaskManagerBLM.Droid.Sources.Utils;
using System.Collections.Generic;
using WhatMessenger.Droid.ChatDetailView;
using System;
using WhatMessenger.Droid.Engine;
using WhatMessenger.Model.RequestModels;
using Square.Picasso;
using WhatMessenger.Model.Constants;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class MainActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_main;
        Android.Support.V7.Widget.Toolbar toolbar;
        Android.Support.V4.Widget.DrawerLayout drawer;
        NavigationView leftNav;

        TabLayout tabLayout;
        TabWithBadge tabChat, tabContact, tabGroup;
        ImageView imgNavProfile;
        TextView txtNavName;
        public enum FRAGMENT_TYPE
        {
            FRAGMENT_EMPTY = -1, FRAGMENT_CHAT = 0, FRAGMENT_GROUP = 1,
            FRAGMENT_CONTACT, FRAGMENT_CALL, FRAGMENT_STATUS, FRAGMENT_ACCOUNT,
            FRAGMENT_SETTING, FRAGMENT_ABOUT, FRAGMENT_FEEDBACK
        };

        public int[] TOOLBAR_TITLE = {
            int.MinValue, Resource.String.chats, Resource.String.group, Resource.String.contact, Resource.String.status, Resource.String.account, Resource.String.setting, Resource.String.about, Resource.String.feedback
        };
        public FRAGMENT_TYPE CURRENT_FRAGMENT { get; set; }
        Dictionary<string, string> StringResource;
        //IMenuItem menuSearch;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            StringResource = this.MyApplication.CurrentLangSetting.GetStringResourceContents();
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            drawer = FindViewById<Android.Support.V4.Widget.DrawerLayout>(Resource.Id.drawer_layout);
            Android.Support.V7.App.ActionBarDrawerToggle toggle = new Android.Support.V7.App.ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();
            leftNav = FindViewById<NavigationView>(Resource.Id.nav_view);
            leftNav.NavigationItemSelected += LeftNav_NavigationItemSelected;

            InitMenuTitle();
            tabLayout = FindViewById<TabLayout>(Resource.Id.tabContent);
            InitTabLayout(); 
            CURRENT_FRAGMENT = FRAGMENT_TYPE.FRAGMENT_EMPTY;
            SetFragment(FRAGMENT_TYPE.FRAGMENT_CHAT);
            SetMeOnline();
        }

        void SetMeOnline()
        {
            var strUserId = Convert.ToString(MyApplication.Me.USERID);
            OnlineStatusRequest model = new OnlineStatusRequest() { IsOnline = true, UserId = strUserId };
            EngineService.EngineInstance.SetMeOnlineStatus(model);
        }

        protected override void OnStart()
        {
            base.OnStart();
            RunOnUiThread(() => {
                UpdateNavHeaderView();
            });
            SetMeOnline();
        }
 
        protected override void OnStop()
        {
            base.OnStop();

        }

        #region Navigation handler region
        void InitMenuTitle()
        {
            var HeaderView = leftNav.GetHeaderView(0);
            imgNavProfile = HeaderView.FindViewById<ImageView>(Resource.Id.navProfileImg);
            txtNavName = HeaderView.FindViewById<TextView>(Resource.Id.txtNavName);
            
            UpdateNavHeaderView();
            leftNav.Menu.FindItem(Resource.Id.home).SetTitle(StringResource.GetValueOrDefault("menuHome"));
            leftNav.Menu.FindItem(Resource.Id.navStatus).SetTitle(StringResource.GetValueOrDefault("menuStatus"));
            leftNav.Menu.FindItem(Resource.Id.navAccount).SetTitle(StringResource.GetValueOrDefault("menuAccount"));
            leftNav.Menu.FindItem(Resource.Id.navSetting).SetTitle(StringResource.GetValueOrDefault("menuSetting"));
        }

        void UpdateNavHeaderView()
        {
            txtNavName.Text = MyApplication.Me.NAME;
            if(string.IsNullOrEmpty(MyApplication.Me.PHOTO))
            {
                Picasso.With(this).Load(Resource.Drawable.logo).Into(imgNavProfile);
            }
            else
            {
                var URL = ServerURL.BaseURL + MyApplication.Me.PHOTO;
                Picasso.With(this).Load(URL).Into(imgNavProfile);
            }
        }
        #endregion

        #region TabFragment
        void InitTabLayout()
        {
            if (tabLayout.ChildCount > 1)
                return;
            tabChat = new TabWithBadge(this, tabLayout.NewTab(), StringResource.GetValueOrDefault("menuChats"), 0);
            tabLayout.AddTab(tabChat.TabView);

            tabGroup = new TabWithBadge(this, tabLayout.NewTab(), StringResource.GetValueOrDefault("menuGroup"), 0);
            tabLayout.AddTab(tabGroup.TabView);

            tabContact = new TabWithBadge(this, tabLayout.NewTab(), StringResource.GetValueOrDefault("menuContact"), 0);
            tabLayout.AddTab(tabContact.TabView);

            tabContact = new TabWithBadge(this, tabLayout.NewTab(), StringResource.GetValueOrDefault("menuCalls"), 0);
            tabLayout.AddTab(tabContact.TabView);

            tabLayout.TabSelected += TabLayout_TabSelected;
            tabLayout.TabReselected += TabLayout_TabReselected;
        }

        public void UpdateChatTabLayout(int notificationCount)
        {
            tabChat.SetBadgeCount(notificationCount);
        }

        public void UpdateGroupTabLayout(int notificationCount)
        {
            tabGroup.SetBadgeCount(notificationCount);
        }

        public void UpdateContactTabLayout(int notificationCount)
        {
            tabContact.SetBadgeCount(notificationCount);
        }

        public void SetTabAndFragment(FRAGMENT_TYPE FragmentPosition)
        {
            switch(FragmentPosition)
            {
                case FRAGMENT_TYPE.FRAGMENT_CALL:
                    SetFragment(FRAGMENT_TYPE.FRAGMENT_CALL);
                    GoTab(3);
                    tabLayout.SetScrollPosition(3, 0, true);
                    break;
                case FRAGMENT_TYPE.FRAGMENT_CHAT:
                    SetFragment(FRAGMENT_TYPE.FRAGMENT_CHAT);
                    GoTab(0);
                    tabLayout.SetScrollPosition(0, 0, true);
                    break;
                case FRAGMENT_TYPE.FRAGMENT_CONTACT:
                    SetFragment(FRAGMENT_TYPE.FRAGMENT_CONTACT);
                    GoTab(2);
                    tabLayout.SetScrollPosition(2, 0, true);
                    break;
                case FRAGMENT_TYPE.FRAGMENT_GROUP:
                    SetFragment(FRAGMENT_TYPE.FRAGMENT_GROUP);
                    GoTab(1);
                    tabLayout.SetScrollPosition(1, 0, true);
                    break;
            }
        }

        private void GoTab(int v)
        {
            var tab = tabLayout.GetTabAt(v);
            tab.Select();
        }

        void TabLayout_TabSelected(object sender, TabSelectedEventArgs e)
        {
            switch (e.Tab.Position)
            {
                case 0:
                    SetFragment(FRAGMENT_TYPE.FRAGMENT_CHAT);
                    break;
                case 1:
                    SetFragment(FRAGMENT_TYPE.FRAGMENT_GROUP);
                    break;
                case 2:
                    SetFragment(FRAGMENT_TYPE.FRAGMENT_CONTACT);
                    break;
                case 3:
                    SetFragment(FRAGMENT_TYPE.FRAGMENT_CALL);
                    break;
            }
            CloseDrawer();
        }

        void TabLayout_TabReselected(object sender, TabReselectedEventArgs e)
        {
            CloseDrawer();
        }

        void SetVisibilityTabLayout(bool IsShow)
        {
            tabLayout.Visibility = IsShow?ViewStates.Visible:ViewStates.Gone;
        }

        #endregion
        #region Fragment attach, manage
        void LeftNav_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            CloseDrawer();
            switch (e.MenuItem.ItemId)
            {
                case Resource.Id.home:
                    SetFragment(FRAGMENT_TYPE.FRAGMENT_CHAT);
                    SetVisibilityTabLayout(true);
                    tabLayout.SetScrollPosition(0, 0, true);
                    break;
                //case Resource.Id.navFavorite:
                    //SetFragment(FRAGMENT_TYPE.FRAGMENT_FAVOR);
                    //SetVisibilityTabLayout(false);
                    //break;
                case Resource.Id.navStatus:
                    SetFragment(FRAGMENT_TYPE.FRAGMENT_STATUS);
                    SetVisibilityTabLayout(false);
                    break;
                case Resource.Id.navAccount:
                    SetFragment(FRAGMENT_TYPE.FRAGMENT_ACCOUNT);
                    SetVisibilityTabLayout(false);
                    break;
                case Resource.Id.navSetting:
                    SetFragment(FRAGMENT_TYPE.FRAGMENT_SETTING);
                    SetVisibilityTabLayout(false);
                    break;
            }
        }

        void SetFragment(FRAGMENT_TYPE FragmentType)
        {
            if (FragmentType == CURRENT_FRAGMENT)
                return;
            CURRENT_FRAGMENT = FragmentType;
            SetToolbarTitle(GetString(TOOLBAR_TITLE[(int)FragmentType + 1]));
            Android.Support.V4.App.Fragment f = null;
            switch (FragmentType)
            {
                case FRAGMENT_TYPE.FRAGMENT_CHAT:
                    SetToolbarTitle(StringResource.GetValueOrDefault("menuChats"));
                    ShowHideSearchIcon(true);
                    f = ChatListFragment.GetInstance();
                    break;
                case FRAGMENT_TYPE.FRAGMENT_CALL:
                    SetToolbarTitle(StringResource.GetValueOrDefault("menuCalls"));
                    ShowHideSearchIcon(true);
                    f = CallListFragment.GetInstance();
                    break;
                case FRAGMENT_TYPE.FRAGMENT_ABOUT:
                    ShowHideSearchIcon(false);
                    break;
                case FRAGMENT_TYPE.FRAGMENT_EMPTY:
                    ShowHideSearchIcon(false);
                    break;
                //case FRAGMENT_TYPE.FRAGMENT_FAVOR:
                    //SetToolbarTitle(StringResource.GetValueOrDefault("menuFavorite"));
                    //ShowHideSearchIcon(false);
                    //f = FavoriteFragment.GetInstance();
                    //break;
                case FRAGMENT_TYPE.FRAGMENT_GROUP:
                    SetToolbarTitle(StringResource.GetValueOrDefault("menuGroup"));
                    ShowHideSearchIcon(true);
                    f = GroupListFragment.GetInstance();
                    break;
                case FRAGMENT_TYPE.FRAGMENT_STATUS:
                    SetToolbarTitle(StringResource.GetValueOrDefault("menuStatus"));
                    ShowHideSearchIcon(false);
                    f = StatusFragment.GetInstance();
                    break;
                case FRAGMENT_TYPE.FRAGMENT_ACCOUNT:
                    SetToolbarTitle(StringResource.GetValueOrDefault("menuAccount"));
                    ShowHideSearchIcon(false);
                    f = AccountFragment.GetInstance();
                    break;
                case FRAGMENT_TYPE.FRAGMENT_CONTACT:
                    SetToolbarTitle(StringResource.GetValueOrDefault("menuContact"));
                    ShowHideSearchIcon(true);
                    f = ContactListFragment.GetInstance();
                    break;
                case FRAGMENT_TYPE.FRAGMENT_SETTING:
                    SetToolbarTitle(StringResource.GetValueOrDefault("menuSetting"));
                    ShowHideSearchIcon(false);
                    f = SettingsFragment.GetInstance();
                    break;
                case FRAGMENT_TYPE.FRAGMENT_FEEDBACK:
                    break;
                default:
                    break;
            }

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
                                      .Replace(Resource.Id.mainContent, f)
                                      .Commit();
            }
        }
        #endregion

        #region interface methods as parentActivity
        public void SetToolbarTitle(string value)
        {
            SupportActionBar.Title = value;
        }

        public void CloseDrawer()
        {
            if (drawer.IsDrawerOpen(GravityCompat.Start))
                drawer.CloseDrawer(GravityCompat.Start);
        }
        #endregion

        protected override void OnDestroy()
        {
            tabLayout.TabSelected -= TabLayout_TabSelected;
            tabLayout.TabReselected -= TabLayout_TabReselected;
          
            if (EngineService.EngineInstance != null && MyApplication.Me != null)
            {
                var strUserId = Convert.ToString(MyApplication.Me.USERID);
                OnlineStatusRequest model = new OnlineStatusRequest() { IsOnline = false, UserId = strUserId };
                EngineService.EngineInstance.SetMeOnlineStatus(model);
                EngineService.EngineInstance.StopThis();
            }
            base.OnDestroy();
        }

        protected override void OnPause()
        {
            base.OnPause();
            //if (EngineService.EngineInstance != null && MyApplication.Me != null)
            //{
            //    var strUserId = Convert.ToString(MyApplication.Me.USERID);
            //    OnlineStatusRequest model = new OnlineStatusRequest() { IsOnline = false, UserId = strUserId };
            //    EngineService.EngineInstance.SetMeOnlineStatus(model);
            //    EngineService.EngineInstance.StopThis();
            //}
        }
        #region menu manage part
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main, menu);
            //menuSearch = menu.FindItem(Resource.Id.menuSearch);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return true;
        }

        public void ShowHideSearchIcon(bool value)
        {
            //if(menuSearch != null)
                //menuSearch.SetVisible(value);
        }

        public void ChangeLanguage()
        {
            this.MyApplication.InitLangResource(!string.Equals(PreferenceUtils.readString(this.BaseContext, PreferenceUtils.LANG), @"CH") ? @"EN" : @"CH");
            leftNav.Menu.GetItem(0).SetChecked(true);
            Recreate();
        }
        #endregion
    }
}
