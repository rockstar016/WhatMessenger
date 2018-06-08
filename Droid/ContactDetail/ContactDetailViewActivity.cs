
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using API.Models.RequestModels;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Rock.Utils;
using Square.Picasso;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Droid.Engine;
using WhatMessenger.Model.BaseModel;
using WhatMessenger.Model.Constants;
using WhatMessenger.Model.RequestModels;
using WhatMessenger.ViewModel;
using WhatMessenger.ViewModel.Utils;

namespace WhatMessenger.Droid.ContactDetail
{
    [Activity(Label = "", Theme = "@style/AppTheme.NoActionBar")]
    public class ContactDetailViewActivity : BaseActivity
    {
        public const string DETAIL_VIEW_USER_ID = "USER_ID";
        Android.Support.V7.Widget.Toolbar toolbar;
        protected override int LayoutResource => Resource.Layout.activity_profile;
        TextView txtStatusLastUpdateDate, txtStatusContent, txtUserName, txtUserPhone, txtUserEmail;
        ImageView imgProfile, imgPhone, imgEmail;
        Button btBlock, btChat, btCall;
        FloatingActionButton fab;
        ChatListViewModel ChatViewModel;
        ContactListViewModel ContactViewModel;

        int ViewContactUserId = 0;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            ChatViewModel = EngineService.EngineInstance.ChatListViewModel;
            ContactViewModel = EngineService.EngineInstance.ContactListViewModel;

            base.OnCreate(savedInstanceState);
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            imgProfile = FindViewById<ImageView>(Resource.Id.imgProfile);

            ViewContactUserId = Intent.GetIntExtra(DETAIL_VIEW_USER_ID, 0);
            txtStatusLastUpdateDate = FindViewById<TextView>(Resource.Id.txtStatusLastUpdate);
            txtStatusContent = FindViewById<TextView>(Resource.Id.txtStatusContent);
            txtUserName = FindViewById<TextView>(Resource.Id.txtUserName);
            txtUserPhone = FindViewById<TextView>(Resource.Id.txtUserPhone);
            txtUserEmail = FindViewById<TextView>(Resource.Id.txtUserEmail);
            imgPhone = FindViewById<ImageView>(Resource.Id.imgPhone);
            imgEmail = FindViewById<ImageView>(Resource.Id.imgEmail);
            btBlock = FindViewById<Button>(Resource.Id.btBlock);
            fab = FindViewById<FloatingActionButton>(Resource.Id.fab);

            btChat = FindViewById<Button>(Resource.Id.btStartChat);
            btCall = FindViewById<Button>(Resource.Id.btCall);
            btChat.Click += BtChat_Click;
        }

        void BtChat_Click(object sender, EventArgs e)
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                ShowLoadingDialog("Initializing");
                if(ChatViewModel.CurrentlyOpenDTO != null && (ChatViewModel.CurrentlyOpenDTO.OtherUserId == ContactViewModel.CurrentOpenContactDTO.USERID))
                {
                    Finish();
                }
                else
                {
                    if (ContactViewModel.CurrentOpenContactDTO != null) 
                        ChatViewModel.OpenCreateChatEntrySocketCommand.Execute(ContactViewModel.CurrentOpenContactDTO);    
                }
            }
            else
                DialogUtils.ShowOKDialog(this, @"waring", @"No internet connection");
        }


        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(string.Equals(e.PropertyName, nameof(ChatListViewModel.CurrentlyOpenDTO)))
            {
                HideLoadingDialog();
                if(ChatViewModel.CurrentlyOpenDTO != null)
                {
                    var IntentDetail = new Intent(this, typeof(ChatDetailView.ChatDetailView));
                    StartActivity(IntentDetail);    
                }
            }
            else if(string.Equals(e.PropertyName, nameof(ChatListViewModel.IsBusy)))
            {
                if(ChatViewModel.IsBusy)
                {
                    ShowLoadingDialog("Initializing");
                }
                else
                {
                    HideLoadingDialog();
                }
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            if(ChatViewModel == null)
            {
                EngineService.EngineInstance.ChatListViewModel = new ChatListViewModel();
                ChatViewModel = new ChatListViewModel();
            }

            if(ContactViewModel == null)
            {
                EngineService.EngineInstance.ContactListViewModel = new ContactListViewModel();
                ContactViewModel = new ContactListViewModel();
            }

            if(ChatViewModel.PrivateChatEntryCollection.Count == 0 && CrossConnectivity.Current.IsConnected)
            {
                GetPrivateChatEntryRequest request = new GetPrivateChatEntryRequest()
                {
                    MY_USER_ID = MyApplication.Me.USERID,
                    TOKEN = MyApplication.Me.TOKEN
                };
                ChatViewModel.LoadAllChatEntryItemCommand.Execute(request);
            }
            ChatViewModel.PropertyChanged += ViewModel_PropertyChanged;
            if (ContactViewModel.CurrentOpenContactDTO == null)
                ContactViewModel.LoadContactItemCommand.Execute(new GetProfileRequest() { TOKEN = MyApplication.Me.TOKEN, USERID = Convert.ToString(ViewContactUserId) });
            else
                SetDataContent();
            ContactViewModel.PropertyChanged += ContactViewModel_PropertyChanged;
        }

        void ContactViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(string.Equals(e.PropertyName, nameof(ContactListViewModel.IsBusy)))
            {
                    RunOnUiThread(() =>
                    {
                        if (ContactViewModel.IsBusy)
                        {
                            ShowLoadingDialog("Loading information");
                        }
                        else
                        {
                            HideLoadingDialog();
                        }
                    });
            }
            else if(string.Equals(e.PropertyName, nameof(ContactListViewModel.CurrentOpenContactDTO)))
            {
                SetDataContent();
            }
        }


        protected override void OnStop()
        {
            base.OnStop();
            ChatViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            ContactViewModel.PropertyChanged -= ContactViewModel_PropertyChanged;
        }

		protected override void OnDestroy()
		{
            base.OnDestroy();
            ContactViewModel.CurrentOpenContactDTO = null;
		}

		public void SetToolbarTitle(string title)
        {

            RunOnUiThread(() =>
            {
                if (string.IsNullOrEmpty(title))
                {
                    SupportActionBar.Title = string.Empty;
                    toolbar.Title = string.Empty;
                }
                else
                {
                    SupportActionBar.Title = title.Trim();
                    toolbar.Title = title.Trim();
                }
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
            });
        }

        void SetDataContent()
        {
            var CurrentModel = ContactViewModel.CurrentOpenContactDTO;
            SetToolbarTitle(CurrentModel == null ? "" : CurrentModel.NAME);
            txtUserName.Text = CurrentModel == null ? "" :CurrentModel.NAME.Trim();
            txtUserEmail.Text = CurrentModel == null ? "" :CurrentModel.EMAIL.Trim();
            if(CurrentModel == null || string.IsNullOrEmpty(CurrentModel.PHONE))
            {
                txtUserPhone.Text = @"No phone number";
            }
            else
            {
                txtUserPhone.Text = CurrentModel.PHONE.Trim();
            }

            if (CurrentModel == null || string.IsNullOrEmpty(CurrentModel.PIC))
            {
                Picasso.With(this).Load(Resource.Drawable.female_placeholder).Into(imgProfile);
            }
            else
            {
                Picasso.With(this).Load(ServerURL.BaseURL + CurrentModel.PIC).Into(imgProfile);
            }

            if (CurrentModel == null)
            {
                txtStatusContent.Text = "Hi, there, I am using NightOwl";
                txtStatusLastUpdateDate.Text = string.Format("{0}: {1}", "Member From: ", "");
            }
            else
            {
                var titleContent = CurrentModel.USER_STATUS_TITLE;
                txtStatusContent.Text = string.IsNullOrEmpty(titleContent)?"Hi, there, I am using NightOwl":titleContent;
                txtStatusLastUpdateDate.Text = string.Format("{0}: {1}", "Member From: ", DateConverter.GetDateTimeFromUnixTimeStamp(CurrentModel.USER_UPDATED_AT).ToString("yyyy-MM-dd hh:mm"));
            }
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
