
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
using Rock.Utils;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Droid.Engine;
using WhatMessenger.Model;
using WhatMessenger.Model.BaseModel;
using WhatMessenger.Model.RequestModels;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.Fragments
{
    public class ChatListFragment : MainBaseFragment
    {
        public static ChatListFragment GetInstance() => new ChatListFragment { Arguments = new Bundle() };
        RecyclerView recycler;
        FloatingActionButton fabAdd;
        SwipeRefreshLayout swipeRefresh;
        ChatListViewModel ChatListViewModel;
        ChatListAdapter Adapter;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (EngineService.EngineInstance.ChatListViewModel == null)
                EngineService.EngineInstance.ChatListViewModel = new ChatListViewModel();
            ChatListViewModel = EngineService.EngineInstance.ChatListViewModel;

            var rootView = inflater.Inflate(Resource.Layout.fragment_chat, container, false);
            recycler = rootView.FindViewById<RecyclerView>(Resource.Id.recycleChatHistory);
            fabAdd = rootView.FindViewById<FloatingActionButton>(Resource.Id.fabAddChat);
            swipeRefresh = rootView.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefresh);
            InitSwipeRefreshLayout(swipeRefresh);

            recycler.HasFixedSize = true;
            recycler.SetLayoutManager(new LinearLayoutManager(this.Context, LinearLayoutManager.Vertical, false));
            recycler.SetItemAnimator(new DefaultItemAnimator());

            Adapter = new ChatListAdapter(Activity, ChatListViewModel);
            recycler.SetAdapter(Adapter);

            InitRecyclerScrollListener(recycler, (e) => {
                if (e && fabAdd.IsShown)
                {
                    fabAdd.Hide();
                }
                else if (!e && !fabAdd.IsShown)
                {
                    fabAdd.Show();
                }
            });

            fabAdd.Click += (sender, e) => {
                ParentActivity.SetTabAndFragment(MainActivity.FRAGMENT_TYPE.FRAGMENT_CONTACT);
            };
            return rootView;
        }

        void SwipeRefresherHandle(object sender, EventArgs e)
        {
            if (Plugin.Connectivity.CrossConnectivity.Current.IsConnected)
            {
                GetPrivateChatEntryRequest model = new GetPrivateChatEntryRequest()
                {
                    MY_USER_ID = ParentActivity.MyApplication.Me.USERID,
                    TOKEN = ParentActivity.MyApplication.Me.TOKEN
                };
                ChatListViewModel.LoadAllChatEntryItemCommand.Execute(model);
            }
            else
            {
                DialogUtils.ShowOKDialog(ParentActivity, @"Warning", @"No Internet Connection");
            }
        }

        public override void OnStart()
        {
            base.OnStart();
      
            swipeRefresh.Refresh += SwipeRefresherHandle;

            ChatListViewModel.PropertyChanged += ViewModel_PropertyChanged;
            Adapter.ItemClick += Adapter_ItemClick;
            Adapter.ItemLongClick += Adapter_ItemLongClick;
            ChatListViewModel.PrivateChatHistoryCollection.Clear();
            if (ChatListViewModel.PrivateChatEntryCollection.Count() == 0)
            {
                GetPrivateChatEntryRequest model = new GetPrivateChatEntryRequest()
                {
                    MY_USER_ID = ParentActivity.MyApplication.Me.USERID,
                    TOKEN = ParentActivity.MyApplication.Me.TOKEN
                };
                if (Plugin.Connectivity.CrossConnectivity.Current.IsConnected)
                {
                    ChatListViewModel.LoadAllChatEntryItemCommand.Execute(model);
                }
                else
                {
                    DialogUtils.ShowOKDialog(ParentActivity, @"Warning", @"No Internet Connection");
                }
            }
        }

        private void Adapter_ItemLongClick(object sender, RecyclerClickEventArgs e)
        {
            //var selectedItem = ChatListViewModel.Items[e.Position];
            ////because item is not updated till now.. 
            //e.View.SetBackgroundResource(Resource.Color.grayGapColor);
            //var ClonedItem = new ChatEntryDTO() { Id = selectedItem.EntryID, LastUpdateDate = selectedItem.LastUpdateDate,  UnreadMessageCount = 0 };
            //ChatListViewModel.UpdateChatItemStatus.Execute(ClonedItem);
        }

        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ChatListViewModel.IsBusy):
                    Activity.RunOnUiThread(() =>
                    {
                        if (ChatListViewModel.IsBusy && !swipeRefresh.Refreshing)
                        {
                            swipeRefresh.Refreshing = true;
                        }
                        else if (!ChatListViewModel.IsBusy)
                        {
                            swipeRefresh.Refreshing = false;
                        }
                    });
                    break;
                case nameof(ChatListViewModel.NotificationCount):
                    ParentActivity.UpdateChatTabLayout(ChatListViewModel.NotificationCount);
                    break;
            }
        }


        private void Adapter_ItemClick(object sender, RecyclerClickEventArgs e)
        {
            var ClickedItem = ChatListViewModel.PrivateChatEntryCollection[e.Position];
            ClickedItem.UnreadMessageCount = 0;
            ChatListViewModel.CurrentlyOpenDTO = ClickedItem;
            ChatListViewModel.RemoveUnreadCountCommand.Execute(null);
            //ChatListViewModel.UpdateChatItemStatus.Execute(ClickedItem);
            var ChatIntent = new Intent(this.ParentActivity, typeof(ChatDetailView.ChatDetailView));
            StartActivity(ChatIntent);
        }

        public override void OnStop()
        {
            base.OnStop();
            swipeRefresh.Refresh -= SwipeRefresherHandle;
            ChatListViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            Adapter.ItemClick -= Adapter_ItemClick;
            Adapter.ItemLongClick -= Adapter_ItemLongClick;
        }

    }
}
