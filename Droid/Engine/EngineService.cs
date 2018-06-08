
using System;

using Android.App;
using Android.Content;
using Android.OS;
using WhatMessenger.Model.RequestModels;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.Engine
{
    [Service(Label = "EngineService", Exported = true)]
    [IntentFilter(new String[] { "com.rock.WhatMessenger.EngineService" })]
    public class EngineService : Service
    {
        public static EngineService EngineInstance { get; set; }
        public ContactListViewModel ContactListViewModel { get; set; }
        public ChatListViewModel ChatListViewModel { get; set; }
        public GroupListViewModel GroupListViewModel { get; set; }
        public ProfileViewModel ProfileViewModel { get; set; }

        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            if(EngineInstance == null)
            {
                EngineInstance = this;
            }
            InitViewModels();
            StartSockets();
            return StartCommandResult.NotSticky;
        }

        public void StopThis()
        {
            var myApp = Application as MainApplication;
            if (myApp.Me != null) myApp.Me = null;
            ChatListViewModel.StopSocketCommunication.Execute(null);
            ContactListViewModel = null;
            ChatListViewModel = null;
            EngineInstance = null;
            StopSelf();
        }

        void InitViewModels()
        {
            if (ContactListViewModel == null) ContactListViewModel = new ContactListViewModel();
            if (ChatListViewModel == null) ChatListViewModel = new ChatListViewModel();
            if (GroupListViewModel == null) GroupListViewModel = new GroupListViewModel();
            if (ProfileViewModel == null) ProfileViewModel = new ProfileViewModel();
        }

        public void SetMeOnlineStatus(OnlineStatusRequest value)
        {
            if (ContactListViewModel == null) ContactListViewModel = new ContactListViewModel();
            ContactListViewModel.SetMeOnlineStatusCommand.Execute(value);
        }

        void StartSockets()
        {
            try
            {
                var myApp = Application as MainApplication;
                if (myApp.Me == null) return;
                object x = myApp.Me.USERID;
                ChatListViewModel.StartChatSocketCommand.Execute(x);
                ContactListViewModel.StartChatSocketCommand.Execute(x);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }


    }
}
