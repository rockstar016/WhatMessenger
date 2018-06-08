
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
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
using WhatMessenger.Droid.Engine;
using WhatMessenger.Droid.GroupDetail;
using WhatMessenger.Droid.Helpers;
using WhatMessenger.Droid.Utils;
using WhatMessenger.Model.Constants;
using WhatMessenger.Model.RequestModels;
using WhatMessenger.ViewModel;
using WhatMessenger.ViewModel.Utils;

namespace WhatMessenger.Droid.ChatDetailView
{
    [Activity(Label = "GroupChatDetailView", WindowSoftInputMode = SoftInput.StateHidden)]
    public class GroupChatDetailView : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_chat;
        Android.Support.V7.Widget.Toolbar toolbar;
        public RecyclerView recyclerView;
        GroupDetailAdapter chatAdapter;
        GroupListViewModel ViewModel;
        CircleImageView imgToolbarCharacter;
        TextView txtToolbarUserName, txtToolbarStatus;
        ImageButton fabSend, fabRec, fabAttach;
        EditText txtContent;
        string userListStr;
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

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerChat);
            fabSend = FindViewById<ImageButton>(Resource.Id.fabSend);
            fabSend.Click += FabSend_Click;
            fabRec = FindViewById<ImageButton>(Resource.Id.fabRec);
            fabRec.Click += FabRec_Click;
            txtContent = FindViewById<EditText>(Resource.Id.txtContent);
            txtContent.TextChanged += TxtContent_TextChanged;
            LinearLayoutManager layoutManager = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(layoutManager);
            recyclerView.SetItemAnimator(new DefaultItemAnimator());
            chatAdapter = new GroupDetailAdapter(this, ViewModel, MyApplication.Me);
            chatAdapter.ItemClick += ChatAdapter_ItemClick;
            chatAdapter.CancelClickHandler += ChatAdapter_CancelClickHandler;
            chatAdapter.ItemLongClick += ChatAdapter_ItemLongClick;
            recyclerView.HasFixedSize = true;
            recyclerView.SetAdapter(chatAdapter);
            ShowRecButton(true);
            fabAttach = FindViewById<ImageButton>(Resource.Id.btAttach);
            fabAttach.Click += FabAttach_Click;
        }

        void LoadingMore_Refresh(object sender, EventArgs e)
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                LoadChatHistoryCollectionRequest model;
                if (ViewModel.GroupChatHistoryItemList.Count > 0)
                {
                    model = new LoadChatHistoryCollectionRequest()
                    {
                        LAST_MESSAGE_ID = ViewModel.GroupChatHistoryItemList[0].ID,
                        THREAD_ID = ViewModel.CurrentlyOpenDTO.GROUP_ID,
                        TOKEN = MyApplication.Me.TOKEN
                    };
                }
                else
                {
                    model = new LoadChatHistoryCollectionRequest()
                    {
                        LAST_MESSAGE_ID = -1,
                        THREAD_ID = ViewModel.CurrentlyOpenDTO.GROUP_ID,
                        TOKEN = MyApplication.Me.TOKEN
                    };
                }
                ViewModel.LoadGroupMessageHistoryCommand.Execute(model);
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
#region Chat Item Click Callbacks
        void ChatAdapter_ItemLongClick(object sender, RecyclerClickEventArgs e)
        {
            if (CrossConnectivity.Current.IsConnected)
            {

                if (ViewModel.GroupChatHistoryItemList[e.Position].SENDER_ID == GetMyUserId())
                {


                    DialogUtils.ShowOkCancelDialog(this, "Warning", "Remove Message?",
                                                    () =>
                                                    {
                                                        if (ViewModel.GroupChatHistoryItemList[e.Position].SENDER_ID == GetMyUserId())
                                                            ViewModel.DeleteChatHistoryItemCommand.Execute(e.Position);
                                                    },
                                                   () =>
                                                   {

                                                   });
                }
            }
        }

        void ChatAdapter_ItemClick(object sender, RecyclerClickEventArgs e)
        {
            if (e.Position < 0) return;
            var Item = ViewModel.GroupChatHistoryItemList[e.Position];
            if (Item.TYPE == GlobalConstants.CHAT_HISTORY_ITEM_IMAGE && !string.IsNullOrEmpty(Item.CONTENT))
            {
                var mIntent = new Intent(this, typeof(ImageFullScreenViewActivity));
                mIntent.PutExtra(ImageFullScreenViewActivity.FILE_PATH, Item.CONTENT.Trim());
                StartActivity(mIntent);
            }
            else if (Item.TYPE == GlobalConstants.CHAT_HISTORY_ITEM_PDF && !string.IsNullOrEmpty(Item.CONTENT.Trim()))
            {
                var browserIntent = new Intent(Intent.ActionView);
                browserIntent.SetDataAndType(Android.Net.Uri.Parse(ServerURL.BaseURL + Item.CONTENT), "application/pdf");
                StartActivity(browserIntent);
            }
            else if (Item.TYPE == GlobalConstants.CHAT_HISTORY_ITEM_VIDEO && !string.IsNullOrEmpty(Item.CONTENT.Trim()))
            {
                var playerIntent = new Intent(this, typeof(PlayerActivity));
                playerIntent.PutExtra(PlayerActivity.BUNDLE_VIDEO_URL, ServerURL.BaseURL + Item.CONTENT);
                StartActivity(playerIntent);
            }
            else if (Item.TYPE == GlobalConstants.CHAT_HISTORY_ITEM_AUDIO && !string.IsNullOrEmpty(Item.CONTENT.Trim()))
            {
                var playIndicator = AudioPlayIndicator.GetInstance(Item.CONTENT.Trim());
                playIndicator.Show(SupportFragmentManager, "player");
            }
        }

        void ChatAdapter_CancelClickHandler(object sender, RecyclerClickEventArgs e)
        {
            if (e.Position < 0) return;
            var Item = ViewModel.GroupChatHistoryItemList[e.Position];
            if (Item.ID == 0 && CrossConnectivity.Current.IsConnected)
            {
                ViewModel.StopUploadingOperationCommand.Execute(Item);
            }
        }


#endregion
        void TxtContent_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            ShowRecButton(this.txtContent.Text.Length == 0);
        }

        #region Attachment Region Handlers
        void FabAttach_Click(object sender, EventArgs e)
        {
            var f = SupportFragmentManager.FindFragmentById(Resource.Id.attachToolbox);
            if (f == null)
            {
                AddAttachToolBoxFragment();
            }
        }

        void AddAttachToolBoxFragment()
        {
            var f = AttachToolboxStatus.GetInstance();
            f.ImageChooseHandler += AttachImageChooseHandler;
            f.VideoChooseHandler += AttachVideoChooseHandler;
            f.ImageTakeHandler += AttachImageTakeHandler;
            f.VideoTakeHandler += AttachVideoTakeHandler;
            f.AttachFileHandler += File_AttachFileHandler;
            SupportFragmentManager.BeginTransaction()
                                  .Replace(Resource.Id.attachToolbox, f)
                                  .Commit();
        }

        async void File_AttachFileHandler(object sender, EventArgs e)
        {
            RemoveAttachToolBoxFragment();
            var PermStorage = await CheckNecessaryPermissions(Permission.Storage);
            if (PermStorage && CrossConnectivity.Current.IsConnected)
            {
                try
                {
                    FileData fileData = await CrossFilePicker.Current.PickFile();
                    if (fileData == null) return;
                    var extFile = Path.GetExtension(fileData.FilePath);
                    if (!string.Equals(extFile.ToLower(), ".pdf"))
                    {
                        DialogUtils.ShowOKDialog(this, "Warning", "Only Pdf is available to send");
                        return;
                    }
                    if (ViewModel.BinaryFileToBeSent == null) ViewModel.BinaryFileToBeSent = new List<FileData>();
                    ViewModel.BinaryFileToBeSent.Add(fileData);

                    var model = new AddGroupChatHistoryItemRequest()
                    {
                        SENDER_ID = GetMyUserId(),
                        SENDER_NAME = GetMyUserName(),
                        GROUP_ID = ViewModel.CurrentlyOpenDTO.GROUP_ID,
                        MSG_TYPE = GlobalConstants.CHAT_HISTORY_ITEM_PDF,
                        MSG_CONTENT = ""
                    };
                    ViewModel.SendGroupMessageCommand.Execute(model);
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
            try
            {
                var PermStorage = await CheckNecessaryPermissions(Permission.Storage);
                if (PermStorage)
                {
                    if (!CrossMedia.Current.IsPickPhotoSupported)
                    {
                        DialogUtils.ShowOKDialog(this, @"Warning", @"No gallery available.");
                        return;
                    }
                    if (ViewModel.PhotoFileToBeSent == null) ViewModel.PhotoFileToBeSent = new List<MediaFile>();
                    var mediaFile = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                    {
                        PhotoSize = PhotoSize.Medium
                    });

                    if (mediaFile != null)
                    {
                        ViewModel.PhotoFileToBeSent.Add(mediaFile);
                        var model = new AddGroupChatHistoryItemRequest()
                        {
                            SENDER_ID = GetMyUserId(), 
                            SENDER_NAME = GetMyUserName(),
                            GROUP_ID = ViewModel.CurrentlyOpenDTO.GROUP_ID,
                            MSG_TYPE = GlobalConstants.CHAT_HISTORY_ITEM_IMAGE,
                            MSG_CONTENT = ""
                        };
                        ViewModel.SendGroupMessageCommand.Execute(model);
                    }
                }
                else
                {
                    ViewModel.PhotoFileToBeSent = null;
                }    
            }
            catch (Exception e1)
            {
                Console.WriteLine(e1.ToString());
            }
        }

        async void AttachVideoChooseHandler(object sender, EventArgs e)
        {
            RemoveAttachToolBoxFragment();
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
                    if (new FileInfo(mediaFile.Path).Length / 1048576 > 50)
                    {
                        DialogUtils.ShowOKDialog(this, @"Warning", @"Large File.");
                        return;
                    }

                    ViewModel.PhotoFileToBeSent.Add(mediaFile);
                    var model = new AddGroupChatHistoryItemRequest()
                    {
                        SENDER_ID = GetMyUserId(),
                        SENDER_NAME = GetMyUserName(),
                        GROUP_ID = ViewModel.CurrentlyOpenDTO.GROUP_ID,
                        MSG_TYPE = GlobalConstants.CHAT_HISTORY_ITEM_VIDEO,
                        MSG_CONTENT = ""
                    };

                    ViewModel.SendGroupMessageCommand.Execute(model);
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
            try
            {
                var PermStorage = await CheckNecessaryPermissions(Plugin.Permissions.Abstractions.Permission.Storage);
                var CameraStorage = await CheckNecessaryPermissions(Plugin.Permissions.Abstractions.Permission.Camera);
                if (PermStorage && CameraStorage)
                {
                    if (!CrossMedia.Current.IsCameraAvailable && !CrossMedia.Current.IsTakePhotoSupported)
                    {
                        DialogUtils.ShowOKDialog(this, @"Warning", @"No Camera available.");
                        return;
                    }
                    if (ViewModel.PhotoFileToBeSent == null) ViewModel.PhotoFileToBeSent = new List<MediaFile>();
                    var mediaFile = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                    {
                        Directory = "Group",
                        Name = DateTime.Now.ToString() + ".jpg",
                        PhotoSize = PhotoSize.Large
                    });

                    if (mediaFile != null)
                    {
                        ViewModel.PhotoFileToBeSent.Add(mediaFile);
                        var model = new AddGroupChatHistoryItemRequest()
                        {
                            SENDER_ID = GetMyUserId(),
                            SENDER_NAME = GetMyUserName(),
                            GROUP_ID = ViewModel.CurrentlyOpenDTO.GROUP_ID,
                            MSG_TYPE = GlobalConstants.CHAT_HISTORY_ITEM_IMAGE,
                            MSG_CONTENT = "",
                        };
                        ViewModel.SendGroupMessageCommand.Execute(model);
                    }
                }
                else
                {
                    ViewModel.PhotoFileToBeSent = null;
                }
            }
            catch (Exception e2)
            {
                
            }
        }

        async void AttachVideoTakeHandler(object sender, EventArgs e)
        {
            RemoveAttachToolBoxFragment();
            var PermStorage = await CheckNecessaryPermissions(Plugin.Permissions.Abstractions.Permission.Storage);
            var PermMic = await CheckNecessaryPermissions(Plugin.Permissions.Abstractions.Permission.Microphone);
            var PermCamera = await CheckNecessaryPermissions(Permission.Camera);
            if (PermStorage && PermCamera && PermMic && CrossConnectivity.Current.IsConnected)
            {
                if (!CrossMedia.Current.IsTakeVideoSupported)
                {
                    DialogUtils.ShowOKDialog(this, @"Warning", @"No video available.");
                    return;
                }
                if (ViewModel.PhotoFileToBeSent == null) ViewModel.PhotoFileToBeSent = new List<MediaFile>();
                var mediaFile = await CrossMedia.Current.TakeVideoAsync(new StoreVideoOptions()
                {
                    Directory = "Private",
                    Name = string.Format("{0}_{1}.mp4", DateConverter.GetUnixTimeSpanFromDate(DateTime.Now), GetMyUserId())
                });

                if (mediaFile != null)
                {
                    if (new FileInfo(mediaFile.Path).Length / 1048576 > 50)
                    {
                        DialogUtils.ShowOKDialog(this, @"Warning", @"Large File.");
                        return;
                    }
                    ViewModel.PhotoFileToBeSent.Add(mediaFile);

                    var model = new AddGroupChatHistoryItemRequest()
                    {
                        SENDER_ID = GetMyUserId(),
                        SENDER_NAME = GetMyUserName(),
                        GROUP_ID = ViewModel.CurrentlyOpenDTO.GROUP_ID,
                        MSG_TYPE = GlobalConstants.CHAT_HISTORY_ITEM_VIDEO,
                        MSG_CONTENT = "",
                    };
                    ViewModel.SendGroupMessageCommand.Execute(model);
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

        void FabSend_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtContent.Text.Trim()))
            {
                var model = new AddGroupChatHistoryItemRequest()
                {
                    SENDER_ID = GetMyUserId(),
                    SENDER_NAME = GetMyUserName(),
                    GROUP_ID = ViewModel.CurrentlyOpenDTO.GROUP_ID,
                    MSG_TYPE = GlobalConstants.CHAT_HISTORY_ITEM_TEXT,
                    MSG_CONTENT = txtContent.Text.Trim(),
                };
                ViewModel.SendGroupMessageCommand.Execute(model);
                RunOnUiThread(() =>
                {
                    txtContent.Text = "";
                });
            }
        }

        #region Record button handler

        void ShowRecButton(bool value)
        {
            RunOnUiThread(() =>
            {
                if (value)
                {
                    fabRec.Visibility = ViewStates.Visible;
                    fabSend.Visibility = ViewStates.Invisible;
                }
                else
                {
                    fabRec.Visibility = ViewStates.Invisible;
                    fabSend.Visibility = ViewStates.Visible;
                }
            });
        }

        async void FabRec_Click(object sender, EventArgs e)
        {
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
            if (!string.IsNullOrEmpty(e.FilePath) && CrossConnectivity.Current.IsConnected)
            {
                var filePath = ChatDetailViewUtils.SetNewFileNameOfVoice(e.FilePath);
                ViewModel.AudioFileToBeSent.Add(filePath);

                var model = new AddGroupChatHistoryItemRequest()
                {
                    SENDER_ID = GetMyUserId(),
                    SENDER_NAME = GetMyUserName(),
                    GROUP_ID = ViewModel.CurrentlyOpenDTO.GROUP_ID,
                    MSG_TYPE = GlobalConstants.CHAT_HISTORY_ITEM_AUDIO,
                    MSG_CONTENT = txtContent.Text.Trim(),
                };

                var bArray = File.ReadAllBytes(filePath);
                var fileName = Path.GetFileName(filePath);
                var requestModel = new AddGroupVoiceMessageItemRequest()
                {
                    FilebArray = bArray,
                    FileName = fileName,
                    ItemRequest = model
                };
                ViewModel.SendVoiceMessageCommand.Execute(requestModel);
            }
        }

        #endregion

        protected override void OnStart()
        {
            base.OnStart();
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            ViewModel.GroupChatHistoryItemList.CollectionChanged += GroupChatHistoryItemList_Changed;
            if (ViewModel.CurrentlyOpenDTO == null) 
            {
                Finish();   
            }
            else
            {
                if (ViewModel.GroupChatHistoryItemList.Count == 0)
                {
                    LoadChatHistoryCollectionRequest model = new LoadChatHistoryCollectionRequest()
                    {
                        LAST_MESSAGE_ID = -1,
                        THREAD_ID = ViewModel.CurrentlyOpenDTO.GROUP_ID,
                        TOKEN = MyApplication.Me.TOKEN
                    };
                    ViewModel.LoadGroupMessageHistoryCommand.Execute(model);
                }
                else
                {
                    RunOnUiThread(() =>
                    {
                        if (ViewModel.GroupChatHistoryItemList.Count > 0)
                            recyclerView.ScrollToPosition(ViewModel.GroupChatHistoryItemList.Count);
                        SetContentSupportActionBarItems();
                    });
                }
            }
        }

		protected override void OnPause()
		{
            base.OnPause();
            ViewModel.AlreadyConnected = false;
		}
		
        protected override void OnStop()
        {
            base.OnStop();
            ViewModel.AlreadyConnected = true;
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            ViewModel.GroupChatHistoryItemList.CollectionChanged -= GroupChatHistoryItemList_Changed;
        }

        void GroupChatHistoryItemList_Changed(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                this.chatAdapter.NotifyDataSetChanged();
                if (ViewModel.GroupChatHistoryItemList.Count > 0 && !ViewModel.DonotSlideToEnd)
                {
                    recyclerView.ScrollToPosition(ViewModel.GroupChatHistoryItemList.Count - 1);
                }
            });
        }

        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(string.Equals(e.PropertyName, nameof(GroupListViewModel.CurrentlyOpenDTO)))
            {
                if(ViewModel.CurrentlyOpenDTO == null)
                {
                    Finish();
                }
            }
            else if(string.Equals(e.PropertyName, nameof(ChatListViewModel.IsBusy)))
            {
                if (ViewModel.CanLoadMore && ViewModel.IsBusy && !loadingMore.Refreshing)
                {
                    RunOnUiThread(() => { loadingMore.Refreshing = true; });
                }
                else if (!ViewModel.IsBusy)
                {
                    RunOnUiThread(() => { loadingMore.Refreshing = false; });
                }
            }
        }

        void InitSupportActionBar()
        {
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            txtToolbarUserName = FindViewById<TextView>(Resource.Id.txtName);
            txtToolbarStatus = FindViewById<TextView>(Resource.Id.txtStatus);
            imgToolbarCharacter = FindViewById<CircleImageView>(Resource.Id.imgCharacter);
            imgToolbarCharacter.Click += ImgToolbarCharacter_Click;
        }

        void InitChatViewService()
        {
            if (EngineService.EngineInstance.GroupListViewModel != null)
            {
                ViewModel = EngineService.EngineInstance.GroupListViewModel;
                ViewModel.GroupChatHistoryItemList.Clear();
            }
        }

        void SetContentSupportActionBarItems()
        {
            if (ViewModel.CurrentlyOpenDTO == null) return;
            txtToolbarUserName.Text = ViewModel.CurrentlyOpenDTO.GROUP_NAME;
            if (string.IsNullOrEmpty(ViewModel.CurrentlyOpenDTO.GROUP_AVATAR))
            {
                Picasso.With(this).Load(Resource.Drawable.female_placeholder).Into(imgToolbarCharacter);
            }
            else
            {
                Picasso.With(this).Load(ServerURL.BaseURL + ViewModel.CurrentlyOpenDTO.GROUP_AVATAR).Error(Resource.Drawable.male_placeholder).Into(imgToolbarCharacter);
            }
            this.userListStr = string.Empty;
            foreach (var TempItem in ViewModel.CurrentlyOpenDTO.MEMBER_LIST)
            {
                this.userListStr += TempItem.NAME.Trim() + ",";
            }
            txtToolbarStatus.Text = this.userListStr;
        }

        void ImgToolbarCharacter_Click(object sender, EventArgs e)
        {
            var mIntent = new Intent(this, typeof(EditGroupActivity));
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
                ViewModel.AlreadyConnected = false;
                Finish();
            }
            return true;
        }

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

    }
}
