
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using FFImageLoading;
using Newtonsoft.Json;
using Plugin.AudioRecorder;
using Plugin.Connectivity;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Refractored.Controls;
using Rock.Utils;
using Square.Picasso;
using TaskManagerBLM.Droid.Sources.Utils;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Droid.ChatDetailView.Dialogs;
using WhatMessenger.Droid.ContactDetail;
using WhatMessenger.Droid.Engine;
using WhatMessenger.Droid.Helpers;
using WhatMessenger.Droid.Utils;
using WhatMessenger.Model.BaseModel;
using WhatMessenger.Model.Constants;
using WhatMessenger.Model.RequestModels;
using WhatMessenger.ViewModel;
using WhatMessenger.ViewModel.Utils;

namespace WhatMessenger.Droid.ChatDetailView
{
    [Activity(Label = "ChatDetailView", WindowSoftInputMode =  SoftInput.StateHidden)]
    public class ChatDetailView : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_chat;
        Android.Support.V7.Widget.Toolbar toolbar;
        public RecyclerView recyclerView;
        ChatDetailAdapter chatAdapter;
        ChatListViewModel ViewModel;
        CircleImageView imgToolbarCharacter;
        TextView txtToolbarUserName, txtToolbarStatus;
        ImageButton fabSend, fabRec, fabAttach;
        EditText txtContent;
        ImageView imgBackground;
        SwipeRefreshLayout loadingMore;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            InitChatViewService();
            base.OnCreate(savedInstanceState);
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            InitSupportActionBar();
            InitBackground();
            loadingMore = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeLoadMore);
            loadingMore.SetColorSchemeResources(Resource.Color.colorPrimary, Resource.Color.colorAccent, Resource.Color.colorPrimaryDark);
            loadingMore.Refresh += LoadingMore_Refresh;
            fabSend = FindViewById<ImageButton>(Resource.Id.fabSend);
            fabSend.Click += FabSend_Click;

            txtContent = FindViewById<EditText>(Resource.Id.txtContent);
            txtContent.TextChanged += TxtContent_TextChanged;
            txtContent.FocusChange += TxtContent_FocusChange;

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerChat);
            LinearLayoutManager layoutManager = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(layoutManager);
            recyclerView.SetItemAnimator(new DefaultItemAnimator());
            chatAdapter = new ChatDetailAdapter(this, ViewModel, MyApplication.Me);
            chatAdapter.ItemClick += ChatAdapter_ItemClickAsync;
            chatAdapter.CancelClickHandler += ChatAdapter_CancelClickHandler;
            chatAdapter.ItemLongClick += ChatAdapter_ItemLongClick;
            recyclerView.HasFixedSize = true;
            recyclerView.SetAdapter(chatAdapter);

            fabRec = FindViewById<ImageButton>(Resource.Id.fabRec);
            fabRec.Click += FabRec_Click;
            ShowRecButton(true);

            fabAttach = FindViewById<ImageButton>(Resource.Id.btAttach);
            fabAttach.Click += FabAttach_Click;
        }

        void LoadingMore_Refresh(object sender, EventArgs e)
        {
            if(CrossConnectivity.Current.IsConnected)
            {
                LoadChatHistoryCollectionRequest model;
                if(ViewModel.PrivateChatHistoryCollection.Count > 0)
                {
                    model = new LoadChatHistoryCollectionRequest()
                    {
                        LAST_MESSAGE_ID = ViewModel.PrivateChatHistoryCollection[0].ID,
                        THREAD_ID = ViewModel.CurrentlyOpenDTO.EntryID,
                        TOKEN = MyApplication.Me.TOKEN
                    };   
                }
                else
                {
                    model = new LoadChatHistoryCollectionRequest()
                    {
                        LAST_MESSAGE_ID = -1,
                        THREAD_ID = ViewModel.CurrentlyOpenDTO.EntryID,
                        TOKEN = MyApplication.Me.TOKEN
                    };    
                }
                ViewModel.LoadPrivateMessageCommand.Execute(model);
            }
            else
            {
                DialogUtils.ShowOKDialog(this, @"Warning", @"No Internet Connection");
            }
        }

        private void InitBackground()
        {
            imgBackground = FindViewById<ImageView>(Resource.Id.imgBackground);
            var IsFirstBackground = PreferenceUtils.readBool(this, PreferenceUtils.WALLPAPER);
            imgBackground.SetImageResource(IsFirstBackground ? Resource.Drawable.background_1 : Resource.Drawable.background);
        }

        void InitChatViewService()
        {
            if (EngineService.EngineInstance.ChatListViewModel != null)
            {
                ViewModel = EngineService.EngineInstance.ChatListViewModel;
                ViewModel.IAmTyping = new Model.Chat.TypingStatus()
                {
                    IsTyping = false,
                    MyUserId = ViewModel.CurrentlyOpenDTO.MyUserId,
                    OtherUserId = ViewModel.CurrentlyOpenDTO.OtherUserId,
                    ThreadId = ViewModel.CurrentlyOpenDTO.EntryID
                };
                ViewModel.PrivateChatHistoryCollection.Clear();
            }
        }


        #region Recording Handlers
        void ShowRecButton(bool value)
        {
            RunOnUiThread(() =>
            {
                if (value)
                {
                    fabRec.Visibility = ViewStates.Visible;
                    fabSend.Visibility = ViewStates.Gone;
                }
                else
                {
                    fabRec.Visibility = ViewStates.Gone;
                    fabSend.Visibility = ViewStates.Visible;
                }
            });
        }

        async void FabRec_Click(object sender, EventArgs e)
        {
            if (!ViewModel.CurrentlyOpenDTO.OtherUserActivate)
            {
                DialogUtils.ShowOKDialog(this, "Warning", "User is inactive to receive message");
                return;
            }

            var resultMic = await CheckMicPermission();
            if (!resultMic) return;
            HideRecordingDialog();

            var recDialog = RecordingIndicator.GetInstance();
            recDialog.OnFinishRecording += RecDialog_OnFinishRecording;
            recDialog.Show(SupportFragmentManager, "rec");
        }

        async Task<bool> CheckMicPermission()
        {
            var PermMic = await CheckNecessaryPermissions(Permission.Microphone);
            var PermWrite = await CheckNecessaryPermissions(Permission.Storage);
            return PermMic || PermWrite;
        }

        void HideRecordingDialog()
        {
            var recDialogExist = SupportFragmentManager.FindFragmentByTag("rec");
            if (recDialogExist != null)
            {
                (recDialogExist as RecordingIndicator).Dismiss();
            }
        }

        void RecDialog_OnFinishRecording(object sender, StringEventArgs e)
        {
            if(!string.IsNullOrEmpty(e.FilePath) && CrossConnectivity.Current.IsConnected)
            {
                var filePath = ChatDetailViewUtils.SetNewFileNameOfVoice(e.FilePath);
                ViewModel.AudioFileToBeSent.Add(filePath);
                var ItemModel = new AddPrivateChatHistoryItemRequest()
                {
                    SENDER_ID = ViewModel.CurrentlyOpenDTO.MyUserId,
                    OTHER_USER_ID = ViewModel.CurrentlyOpenDTO.OtherUserId,
                    MSG_TYPE = GlobalConstants.CHAT_HISTORY_ITEM_AUDIO,
                    MSG_CONTENT = "",
                    THREAD_ID = ViewModel.CurrentlyOpenDTO.EntryID
                };
                var bArray = File.ReadAllBytes(filePath);
                var fileName = Path.GetFileName(filePath);
                var requestModel = new AddPrivateVoiceMessageItemRequest()
                {
                    FilebArray = bArray,
                    FileName = fileName,
                    ItemRequest = ItemModel
                };
                ViewModel.SendVoiceMessageCommand.Execute(requestModel);
            }
            else
            {
                DialogUtils.ShowOKDialog(this, @"Warning", @"No Internet Connection");
            }
        }
        #endregion

        #region Adapter Click Callbacks
        void ChatAdapter_ItemLongClick(object sender, RecyclerClickEventArgs e)
        {

            if (CrossConnectivity.Current.IsConnected)
            {
                if (ViewModel.PrivateChatHistoryCollection[e.Position].SENDER_ID == GetMyUserId())
                {
                    DialogUtils.ShowOkCancelDialog(this, "Warning", "Remove Message?",
                    () =>
                    {
                        ViewModel.DeleteChatHistoryItemCommand.Execute(e.Position);
                    },
                    () =>
                    {

                    });
                }
                else
                {
                    ViewModel.PrivateChatHistoryCollection[e.Position].IS_FAVORITE = !ViewModel.PrivateChatHistoryCollection[e.Position].IS_FAVORITE;
                    chatAdapter.NotifyItemChanged(e.Position);
                    ViewModel.FavoriteChatHistoryItemCommand.Execute(e.Position);
                }
            }
        }


        void ChatAdapter_CancelClickHandler(object sender, RecyclerClickEventArgs e)
        {
            if (e.Position < 0) return;
            var Item = ViewModel.PrivateChatHistoryCollection[e.Position];
            if(Item.ID == 0 && CrossConnectivity.Current.IsConnected)
            {
                ViewModel.StopUploadingOperationCommand.Execute(Item);
            }
            else
            {
                DialogUtils.ShowOKDialog(this, @"Warning", @"No Internet Connection");
            }
        }

        void ChatAdapter_ItemClickAsync(object sender, Droid.RecyclerClickEventArgs e)
        {
            if (e.Position < 0) return;
            if (!CrossConnectivity.Current.IsConnected) return;

            var Item = ViewModel.PrivateChatHistoryCollection[e.Position];
            if(Item.TYPE == GlobalConstants.CHAT_HISTORY_ITEM_IMAGE && !string.IsNullOrEmpty(Item.CONTENT))
            {
                var mIntent = new Intent(this, typeof(ImageFullScreenViewActivity));
                mIntent.PutExtra(ImageFullScreenViewActivity.FILE_PATH, Item.CONTENT.Trim());
                StartActivity(mIntent);    
            }
            else if(Item.TYPE == GlobalConstants.CHAT_HISTORY_ITEM_PDF && !string.IsNullOrEmpty(Item.CONTENT.Trim()))
            {
                var browserIntent = new Intent(Intent.ActionView);
                browserIntent.SetDataAndType(Android.Net.Uri.Parse(ServerURL.BaseURL + Item.CONTENT), "application/pdf");
                StartActivity(browserIntent);    
            }
            else if(Item.TYPE == GlobalConstants.CHAT_HISTORY_ITEM_VIDEO && !string.IsNullOrEmpty(Item.CONTENT.Trim()))
            {
                var playerIntent = new Intent(this, typeof(PlayerActivity));
                playerIntent.PutExtra(PlayerActivity.BUNDLE_VIDEO_URL, ServerURL.BaseURL + Item.CONTENT);
                StartActivity(playerIntent);
            }
            else if(Item.TYPE == GlobalConstants.CHAT_HISTORY_ITEM_AUDIO && !string.IsNullOrEmpty(Item.CONTENT.Trim()))
            {
                var playIndicator = AudioPlayIndicator.GetInstance(Item.CONTENT.Trim());
                playIndicator.Show(SupportFragmentManager, "player");
            }
        }
#endregion

        #region Attachment Region Handlers
        void FabAttach_Click(object sender, EventArgs e)
        {
            var f = SupportFragmentManager.FindFragmentById(Resource.Id.attachToolbox);
            if (f == null)
            {
                AddAttachToolBoxFragment();
            }
            else
            {
                RemoveAttachToolBoxFragment();
            }
        }

        void AddAttachToolBoxFragment()
        {
            var f = AttachToolboxStatus.GetInstance();
            f.ImageChooseHandler += AttachImageChooseHandler;
            f.VideoChooseHandler += AttachVideoChooseHandler;
            f.ImageTakeHandler += AttachImageTakeHandler;
            f.VideoTakeHandler += AttachVideoTakeHandler;
            f.AttachFileHandler += AttachFileTakeHandler;
            SupportFragmentManager.BeginTransaction()
                                  .Replace(Resource.Id.attachToolbox, f)
                                  .Commit();
        }

        async void AttachFileTakeHandler(object sender, EventArgs e)
        {
            
            RemoveAttachToolBoxFragment();
            if(!CrossConnectivity.Current.IsConnected)
            {
                DialogUtils.ShowOKDialog(this, @"Warning", @"No Internet Connection");
                return;
            }
            if (!ViewModel.CurrentlyOpenDTO.OtherUserActivate)
            {
                DialogUtils.ShowOKDialog(this, "Warning", "User is inactive to receive message");
                return;
            }
            var PermStorage = await CheckNecessaryPermissions(Permission.Storage);
            if (PermStorage && CrossConnectivity.Current.IsConnected)
            {
                try
                {
                    FileData fileData = await CrossFilePicker.Current.PickFile();
                    if (fileData == null) return;
                    var extFile = Path.GetExtension(fileData.FilePath);
                    if(!string.Equals(extFile.ToLower(), ".pdf"))
                    {
                        DialogUtils.ShowOKDialog(this, "Warning", "Only Pdf is available to send");
                        return;
                    }
                    if (ViewModel.BinaryFileToBeSent == null) ViewModel.BinaryFileToBeSent = new List<FileData>();
                    ViewModel.BinaryFileToBeSent.Add(fileData);
                    var model = new AddPrivateChatHistoryItemRequest()
                    {
                        SENDER_ID = ViewModel.CurrentlyOpenDTO.MyUserId,
                        OTHER_USER_ID = ViewModel.CurrentlyOpenDTO.OtherUserId,
                        MSG_TYPE = GlobalConstants.CHAT_HISTORY_ITEM_PDF,
                        MSG_CONTENT = "",
                        THREAD_ID = ViewModel.CurrentlyOpenDTO.EntryID
                    };
                    ViewModel.SendPrivateMessageCommand.Execute(model);
                }
                catch (Exception ex)
                {
                    
                }
            }
            else
            {
                ViewModel.BinaryFileToBeSent.Clear();
            }
        }

        async void AttachImageChooseHandler(object sender, EventArgs e)
        {
            RemoveAttachToolBoxFragment();
            if (!CrossConnectivity.Current.IsConnected)
            {
                DialogUtils.ShowOKDialog(this, @"Warning", @"No Internet Connection");
                return;
            }
            if (!ViewModel.CurrentlyOpenDTO.OtherUserActivate)
            {
                DialogUtils.ShowOKDialog(this, "Warning", "User is inactive to receive message");
                return;
            }

            var PermStorage = await CheckNecessaryPermissions(Plugin.Permissions.Abstractions.Permission.Storage);
            if(PermStorage && CrossConnectivity.Current.IsConnected)
            {
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    DialogUtils.ShowOKDialog(this, @"Warning", @"No gallery available.");
                    return;
                }
                if (ViewModel.PhotoFileToBeSent == null) ViewModel.PhotoFileToBeSent = new List<MediaFile>();
                var mediaFile = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Large
                });

                if (mediaFile != null)
                {
                    ViewModel.PhotoFileToBeSent.Add(mediaFile);
                    var model = new AddPrivateChatHistoryItemRequest()
                    {
                        SENDER_ID = ViewModel.CurrentlyOpenDTO.MyUserId,
                        OTHER_USER_ID = ViewModel.CurrentlyOpenDTO.OtherUserId,
                        MSG_TYPE = GlobalConstants.CHAT_HISTORY_ITEM_IMAGE,
                        MSG_CONTENT = "",
                        THREAD_ID = ViewModel.CurrentlyOpenDTO.EntryID
                    };
                    ViewModel.SendPrivateMessageCommand.Execute(model);
                }
            }
            else
            {
                ViewModel.PhotoFileToBeSent.Clear();
            }
        }

        async void AttachVideoChooseHandler(object sender, EventArgs e)
        {
            RemoveAttachToolBoxFragment();
            if (!CrossConnectivity.Current.IsConnected)
            {
                DialogUtils.ShowOKDialog(this, @"Warning", @"No Internet Connection");
                return;
            }

            if (!ViewModel.CurrentlyOpenDTO.OtherUserActivate)
            {
                DialogUtils.ShowOKDialog(this, "Warning", "User is inactive to receive message");
                return;
            }

            var PermStorage = await CheckNecessaryPermissions(Plugin.Permissions.Abstractions.Permission.Storage);
            var PermMic = await CheckNecessaryPermissions(Plugin.Permissions.Abstractions.Permission.Microphone);
            if (PermStorage && CrossConnectivity.Current.IsConnected)
            {
                if (!CrossMedia.Current.IsPickVideoSupported)
                {
                    DialogUtils.ShowOKDialog(this, @"Warning", @"No gallery available.");
                    return;
                }
                if (ViewModel.PhotoFileToBeSent == null) ViewModel.PhotoFileToBeSent = new List<MediaFile>();
                var mediaFile = await CrossMedia.Current.PickVideoAsync();

                if (mediaFile != null)
                {
                    if (new FileInfo(mediaFile.Path).Length / 1048576 > 45)
                    {
                        DialogUtils.ShowOKDialog(this, @"Warning", @"Large File.");
                        return;
                    }

                    ViewModel.PhotoFileToBeSent.Add(mediaFile);
                    var model = new AddPrivateChatHistoryItemRequest()
                    {
                        SENDER_ID = ViewModel.CurrentlyOpenDTO.MyUserId,
                        OTHER_USER_ID = ViewModel.CurrentlyOpenDTO.OtherUserId,
                        MSG_TYPE = GlobalConstants.CHAT_HISTORY_ITEM_VIDEO,
                        MSG_CONTENT = "",
                        THREAD_ID = ViewModel.CurrentlyOpenDTO.EntryID
                    };
                    ViewModel.SendPrivateMessageCommand.Execute(model);
                }
            }
            else
            {
                ViewModel.PhotoFileToBeSent.Clear();
            }
        }

        async void AttachImageTakeHandler(object sender, EventArgs e)
        {
            RemoveAttachToolBoxFragment();
            if (!CrossConnectivity.Current.IsConnected)
            {
                DialogUtils.ShowOKDialog(this, @"Warning", @"No Internet Connection");
                return;
            }

            if (!ViewModel.CurrentlyOpenDTO.OtherUserActivate)
            {
                DialogUtils.ShowOKDialog(this, "Warning", "User is inactive to receive message");
                return;
            }


            var PermStorage = await CheckNecessaryPermissions(Plugin.Permissions.Abstractions.Permission.Storage);
            if (PermStorage && CrossConnectivity.Current.IsConnected)
            {
                if (!CrossMedia.Current.IsCameraAvailable && !CrossMedia.Current.IsTakePhotoSupported)
                {
                    DialogUtils.ShowOKDialog(this, @"Warning", @"No Camera available.");
                    return;
                }
                if (ViewModel.PhotoFileToBeSent == null) ViewModel.PhotoFileToBeSent = new List<MediaFile>();
                var mediaFile = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Private",
                    PhotoSize = PhotoSize.Large,
                    Name = string.Format("{0}.jpg", DateConverter.GetUnixTimeSpanFromDate(DateTime.Now))
                });

                if (mediaFile != null)
                {
                    ViewModel.PhotoFileToBeSent.Add(mediaFile);
                    var model = new AddPrivateChatHistoryItemRequest()
                    {
                        SENDER_ID = ViewModel.CurrentlyOpenDTO.MyUserId,
                        OTHER_USER_ID = ViewModel.CurrentlyOpenDTO.OtherUserId,
                        MSG_TYPE = GlobalConstants.CHAT_HISTORY_ITEM_IMAGE,
                        MSG_CONTENT = "",
                        THREAD_ID = ViewModel.CurrentlyOpenDTO.EntryID
                    };
                    ViewModel.SendPrivateMessageCommand.Execute(model);
                }
            }
            else
            {
                ViewModel.PhotoFileToBeSent.Clear();
            }
        }

        async void AttachVideoTakeHandler(object sender, EventArgs e)
        {
            RemoveAttachToolBoxFragment();
            if (!CrossConnectivity.Current.IsConnected)
            {
                DialogUtils.ShowOKDialog(this, @"Warning", @"No Internet Connection");
                return;
            }

            if (!ViewModel.CurrentlyOpenDTO.OtherUserActivate)
            {
                DialogUtils.ShowOKDialog(this, "Warning", "User is inactive to receive message");
                return;
            }

            var PermStorage = await CheckNecessaryPermissions(Plugin.Permissions.Abstractions.Permission.Storage);
            var PermMic = await CheckNecessaryPermissions(Plugin.Permissions.Abstractions.Permission.Microphone);
            var PermCamera = await CheckNecessaryPermissions(Permission.Camera);
            if (PermStorage&& PermCamera && PermMic && CrossConnectivity.Current.IsConnected)
            {
               
                if (!CrossMedia.Current.IsTakeVideoSupported)
                {
                    DialogUtils.ShowOKDialog(this, @"Warning", @"No video available.");
                    return;
                }
                if (ViewModel.PhotoFileToBeSent == null) ViewModel.PhotoFileToBeSent = new List<MediaFile>();
                var mediaFile = await CrossMedia.Current.TakeVideoAsync(new StoreVideoOptions(){
                    Directory = "Private",
                    Name = string.Format("{0}_{1}.mp4", DateConverter.GetUnixTimeSpanFromDate(DateTime.Now), GetMyUserId())
                });

                if (mediaFile != null)
                {
                    if (new FileInfo(mediaFile.Path).Length / 1048576 > 45)
                    {
                        DialogUtils.ShowOKDialog(this, @"Warning", @"Large File.");
                        return;
                    }
                    ViewModel.PhotoFileToBeSent.Add(mediaFile);
                    var model = new AddPrivateChatHistoryItemRequest()
                    {
                        SENDER_ID = ViewModel.CurrentlyOpenDTO.MyUserId,
                        OTHER_USER_ID = ViewModel.CurrentlyOpenDTO.OtherUserId,
                        MSG_TYPE = GlobalConstants.CHAT_HISTORY_ITEM_VIDEO,
                        MSG_CONTENT = "",
                        THREAD_ID = ViewModel.CurrentlyOpenDTO.EntryID
                    };
                    ViewModel.SendPrivateMessageCommand.Execute(model);
                }
            }
            else
            {
                ViewModel.PhotoFileToBeSent.Clear();
            }
        }

        void RemoveAttachToolBoxFragment()
        {
            var f = SupportFragmentManager.FindFragmentById(Resource.Id.attachToolbox);
            if (f != null)
            {
                SupportFragmentManager.BeginTransaction()
                                  .Remove(f)
                                  .Commit();
            }
        }
        #endregion


        void TxtContent_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                DialogUtils.ShowOKDialog(this, @"Warning", @"No Internet Connection");
                return;
            }

            ViewModel.IamTypingCommand.Execute(false);
        }

        void TxtContent_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            ShowRecButton(this.txtContent.Text.Length == 0);
            if (this.txtContent.Text.Length > 0)
            {
                if(CrossConnectivity.Current.IsConnected)
                    ViewModel.IamTypingCommand.Execute(true);
            }
            else
            {
                if (CrossConnectivity.Current.IsConnected)
                    ViewModel.IamTypingCommand.Execute(false);
            }
        }

        void FabSend_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtContent.Text.Trim()))
            {
                if (!ViewModel.CurrentlyOpenDTO.OtherUserActivate)
                {
                    DialogUtils.ShowOKDialog(this, "Warning", "User is inactive to receive message");
                    return;
                }

                var model = new AddPrivateChatHistoryItemRequest()
                {
                    SENDER_ID = ViewModel.CurrentlyOpenDTO.MyUserId,
                    OTHER_USER_ID = ViewModel.CurrentlyOpenDTO.OtherUserId,
                    MSG_TYPE = GlobalConstants.CHAT_HISTORY_ITEM_TEXT,
                    MSG_CONTENT = txtContent.Text.Trim(),
                    THREAD_ID = ViewModel.CurrentlyOpenDTO.EntryID
                };
                if (CrossConnectivity.Current.IsConnected)
                    ViewModel.SendPrivateMessageCommand.Execute(model);
                else
                    DialogUtils.ShowOKDialog(this, @"Warning", @"No Internet Connection");
                RunOnUiThread(() =>
                {
                    txtContent.Text = "";
                });
            }
        }

        void PrivateChatHistoryItemCollection_Changed(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                this.chatAdapter.NotifyDataSetChanged();
                if (ViewModel.PrivateChatHistoryCollection.Count > 0 && !ViewModel.DonotSlideToEnd)
                {
                    recyclerView.ScrollToPosition(ViewModel.PrivateChatHistoryCollection.Count - 1);
                }
            });
        }

        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            
            if(string.Equals(e.PropertyName, nameof(ChatListViewModel.OtherTyping)))
            {
                SetTypingStatus();
            }
            else if(string.Equals(e.PropertyName, nameof(ChatListViewModel.IsBusy)))
            {
                if(ViewModel.CanLoadMore && ViewModel.IsBusy && !loadingMore.Refreshing)
                {
                    RunOnUiThread(() => { loadingMore.Refreshing = true; });
                }
                else if(!ViewModel.IsBusy)
                {
                    RunOnUiThread(() => { loadingMore.Refreshing = false; });
                }
            }
        }

        void SetTypingStatus()
        {
            if (txtToolbarStatus == null) return;
            if (ViewModel.OtherTyping)
            {
                RunOnUiThread(() =>
                {
                    txtToolbarStatus.Visibility = ViewStates.Visible;
                    txtToolbarStatus.Text = "Typing.....";
                });
            }
            else
            {
                RunOnUiThread(() =>
                {
                    txtToolbarStatus.Text = ViewModel.CurrentlyOpenDTO.OtherUserOnlineStatus ? "Online" : "Offline";
                });
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            hideSoftKeyboard();
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            ViewModel.PrivateChatHistoryCollection.CollectionChanged += PrivateChatHistoryItemCollection_Changed;
            //todo load past history with http here
            if (ViewModel.PrivateChatHistoryCollection.Count == 0)
            {
                LoadChatHistoryCollectionRequest model = new LoadChatHistoryCollectionRequest()
                {
                    LAST_MESSAGE_ID = -1,
                    THREAD_ID = ViewModel.CurrentlyOpenDTO.EntryID,
                    TOKEN = MyApplication.Me.TOKEN
                };
                ViewModel.LoadPrivateMessageCommand.Execute(model);
            }
            else
            {
                RunOnUiThread(() =>
                {
                    if (ViewModel.PrivateChatHistoryCollection.Count > 0)
                        recyclerView.ScrollToPosition(ViewModel.PrivateChatHistoryCollection.Count);
                    txtToolbarStatus.Text = ViewModel.CurrentlyOpenDTO.OtherUserOnlineStatus ? "Online" : "Offline";
                });
            }
        }

        protected override void OnStop()
        {
            base.OnStop();
            ViewModel.IamTypingCommand.Execute(false);
            //ViewModel.PrivateChatHistoryCollection.Clear();
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            ViewModel.PrivateChatHistoryCollection.CollectionChanged -= PrivateChatHistoryItemCollection_Changed;
        }

		protected override void OnDestroy()
		{
            base.OnDestroy();
            try
            {
                ViewModel.PhotoFileToBeSent.Clear();
                ViewModel.BinaryFileToBeSent.Clear();
                ViewModel.CurrentlyOpenDTO = null;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
		}

        #region Support Action bar Management
        private void InitSupportActionBar()
        {
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            txtToolbarUserName = FindViewById<TextView>(Resource.Id.txtName);
            txtToolbarStatus = FindViewById<TextView>(Resource.Id.txtStatus);
            imgToolbarCharacter = FindViewById<CircleImageView>(Resource.Id.imgCharacter);

            txtToolbarUserName.Text = ViewModel.CurrentlyOpenDTO.OtherUserName;
            if (string.IsNullOrEmpty(ViewModel.CurrentlyOpenDTO.OtherUserPic))
            {
                Picasso.With(this).Load(Resource.Drawable.female_placeholder).Into(imgToolbarCharacter);
            }
            else
            {
                Picasso.With(this).Load(ServerURL.BaseURL + ViewModel.CurrentlyOpenDTO.OtherUserPic).Error(Resource.Drawable.female_placeholder).Into(imgToolbarCharacter);
            }
            imgToolbarCharacter.Click += ImgToolbarCharacter_Click;
        }

        void ImgToolbarCharacter_Click(object sender, EventArgs e)
        {
            var mIntent = new Intent(this, typeof(ContactDetailViewActivity));
            mIntent.PutExtra(ContactDetailViewActivity.DETAIL_VIEW_USER_ID, ViewModel.CurrentlyOpenDTO.OtherUserId);
            StartActivity(mIntent);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_chat, menu);
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
        #endregion

        #region permission manage
        async Task<bool> CheckNecessaryPermissions(Permission permissionName)
        {
            var permStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(permissionName);
            if (permStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { permissionName });
                permStatus = results[permissionName];
            }
            return await Task.FromResult(permStatus == PermissionStatus.Granted);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        #endregion

        public void hideSoftKeyboard()
        {
            var currentFocus = this.CurrentFocus;
            if (currentFocus != null)
            {
                InputMethodManager inputMethodManager = (InputMethodManager)GetSystemService(Context.InputMethodService);
                inputMethodManager.HideSoftInputFromWindow(currentFocus.WindowToken, HideSoftInputFlags.None);
            }
        }
    }
}
