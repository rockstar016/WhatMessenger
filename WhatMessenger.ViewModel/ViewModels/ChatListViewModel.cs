using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNet.SignalR.Client;
using MvvmHelpers;
using Plugin.FilePicker.Abstractions;
using Plugin.Media.Abstractions;
using WhatMessenger.Model.BaseModel;
using WhatMessenger.Model.Chat;
using WhatMessenger.Model.Constants;
using WhatMessenger.Model.RequestModels;
using WhatMessenger.Services;
using WhatMessenger.ViewModel.Utils;

namespace WhatMessenger.ViewModel
{
    public class ChatListViewModel:BaseViewModel
    {
        public ChatListDataStore DataStore = new ChatListDataStore();

        public ICommand LoadAllChatEntryItemCommand { get; set; }
        public ICommand ClearAllSelectionItem { get; set; }
        public ICommand StartChatSocketCommand { get; set; }
        public ICommand OpenCreateChatEntrySocketCommand { get; set; }
        public ICommand RemoveUnreadCountCommand { get; set; }
        public ICommand SendPrivateMessageCommand { get; set; }
        public ICommand SendVoiceMessageCommand { get; set; }
        public ICommand LoadPrivateMessageCommand { get; set; }
        public ICommand StopSocketCommunication { get; set; }
        public ICommand IamTypingCommand { get; set; }
        public ICommand StopUploadingOperationCommand { get; set; }
        public ICommand ClearAllChatCommand { get; set; }
        public ICommand DeleteChatHistoryItemCommand { get; set; }
        public ICommand FavoriteChatHistoryItemCommand { get; set; }

        public ObservableRangeCollection<ChatEntryDTO> PrivateChatEntryCollection { get; set; }
        public ObservableRangeCollection<ChatHistoryItemDTO> PrivateChatHistoryCollection { get; set; }

        public bool DonotSlideToEnd = false;

        public bool IsFromUpdate = false;
        private int notificationCount = 0;
        public int NotificationCount { get => notificationCount; set => SetProperty(ref notificationCount, value);}
        public TypingStatus IAmTyping;
        bool otherTyping = false;
        public bool OtherTyping { get => otherTyping; set => SetProperty(ref otherTyping, value); }

        ChatEntryDTO currentlyOpenDTO;
        public  ChatEntryDTO CurrentlyOpenDTO { get => currentlyOpenDTO; set => SetProperty(ref currentlyOpenDTO, value); }

        public List<MediaFile> PhotoFileToBeSent { get; set; }
        public List<FileData> BinaryFileToBeSent { get; set; }
        public List<string> AudioFileToBeSent { get; set; }

        public ChatListViewModel()
        {
            Title = "Chat History";

            PhotoFileToBeSent = new List<MediaFile>();
            BinaryFileToBeSent = new List<FileData>();
            AudioFileToBeSent = new List<string>();

            PrivateChatEntryCollection = new ObservableRangeCollection<ChatEntryDTO>();
            PrivateChatHistoryCollection = new ObservableRangeCollection<ChatHistoryItemDTO>();

            LoadAllChatEntryItemCommand = new Command<GetPrivateChatEntryRequest>(async (value) => await ExecuteLoadEntryHistory(value));
            ClearAllSelectionItem = new Command(async () => await ExcuteClearAllSelection());
            StartChatSocketCommand = new Command<object>((userId) => ExecuteChatSocketCommand(userId));
            OpenCreateChatEntrySocketCommand = new Command<ContactDTO>( (obj) =>   ExecuteOpenCreateChatEntrySocketCommand(obj));
            RemoveUnreadCountCommand = new Command(() => ExecuteRemoveUnreadCountCommand());
            SendPrivateMessageCommand = new Command<AddPrivateChatHistoryItemRequest>(async (model) => await ExecuteSendPrivateMessageComand(model));
            LoadPrivateMessageCommand = new Command<LoadChatHistoryCollectionRequest>(async (model) => await ExecuteLoadPastPrivateMessageHistory(model));
            StopSocketCommunication = new Command(() => ExecuteStopSocketCommmunication());
            IamTypingCommand = new Command<bool>((status) => ExecuteIamTypingCommand(status));
            ClearAllChatCommand = new Command<string>(async (token) => await ExecuteClearAllChatCommand(token));
            StopUploadingOperationCommand = new Command<ChatHistoryItemDTO>(async (model) => await ExecuteStopUploadingOperationCommand(model));
            SendVoiceMessageCommand = new Command<AddPrivateVoiceMessageItemRequest>(async (model) => await ExecuteSendPrivateVoiceMessageComand(model));
            DeleteChatHistoryItemCommand = new Command<Int32>((indexValue) => ExecuteRemoveHistoryItem(indexValue));
            FavoriteChatHistoryItemCommand = new Command<Int32>((indexValue) => ExecuteFavoriteChatHistoryItem(indexValue));
        }

        #region Socket Invoke Functions

        async Task ExecuteStopUploadingOperationCommand(ChatHistoryItemDTO DTO)
        {
            var cancelResult = await DataStore.StopUploading(DTO);   
            if(cancelResult)
            {
                PrivateChatHistoryCollection.Remove(DTO);
            }
        }

        void ExecuteIamTypingCommand(bool typingStatus)
        {
            if (IAmTyping.IsTyping == typingStatus)
                return;
            IAmTyping.IsTyping = typingStatus;
            if (SocketManagerDataStore.GetInstance().ChatHubProxy == null)
            {
                IAmTyping.IsTyping = false;
                return;
            }
            SocketManagerDataStore.GetInstance().ChatHubProxy.Invoke("SetTypingStatusPrivateChat", IAmTyping.OtherUserId, IAmTyping.IsTyping);
        }

        void ExecuteRemoveHistoryItem(Int32 indexOfValue)
        {
            if (PrivateChatHistoryCollection == null) return;
            if (PrivateChatHistoryCollection.Count() == 0) return;
            if (PrivateChatHistoryCollection.Count() < indexOfValue) return;
            if (!SocketIsAlive()) return;
            SocketManagerDataStore.GetInstance().ChatHubProxy.Invoke("RemovePrivateChatHistoryItem", PrivateChatHistoryCollection[indexOfValue].ID, CurrentlyOpenDTO.OtherUserId);
        }

        void ExecuteFavoriteChatHistoryItem(Int32 indexOfValue)
        {
            if (PrivateChatHistoryCollection == null) return;
            if (PrivateChatHistoryCollection.Count() == 0) return;
            if (PrivateChatHistoryCollection.Count() < indexOfValue) return;
            if (!SocketIsAlive()) return;
            SocketManagerDataStore.GetInstance().ChatHubProxy.Invoke("FavoritePrivateChatHistoryItem", PrivateChatHistoryCollection[indexOfValue].ID, PrivateChatHistoryCollection[indexOfValue].IS_FAVORITE);
        }

        void ExecuteStopSocketCommmunication()
        {
            SocketManagerDataStore.GetInstance().StopSocketConnection();
        }

        async Task ExecuteLoadPastPrivateMessageHistory(LoadChatHistoryCollectionRequest model)
        {
            if (IsBusy) 
            {
                IsBusy = false;
                return;   
            }
            IsBusy = true;
            var result = await DataStore.GetLoadPrivateChatHistoryCollection(model);
            if(result != null)
            {
                CanLoadMore = result.Count == 100;
                if(PrivateChatHistoryCollection.Count == 0)
                {
                    DonotSlideToEnd = false;
                    PrivateChatHistoryCollection.AddRange(result);
                } 
                else
                {
                    DonotSlideToEnd = true;
                    foreach(var historyItem in result)
                    {
                        PrivateChatHistoryCollection.Insert(0, historyItem);
                    }
                }
            }
            IsBusy = false;
        }

        async Task ExecuteSendPrivateVoiceMessageComand(AddPrivateVoiceMessageItemRequest model)
        {
            if (IsBusy) return;
            if (model.ItemRequest.MSG_TYPE == GlobalConstants.CHAT_HISTORY_ITEM_AUDIO && AudioFileToBeSent.Count == 0) return;
            var emptyDTO = new ChatHistoryItemDTO()
            {
                ID = 0,
                TYPE = model.ItemRequest.MSG_TYPE,
                CONTENT = "",
                CREATED_AT = DateConverter.GetUnixTimeSpanFromDate(DateTime.Now),
                READ_STATUS = false,
                IS_FAVORITE = false,
                SENDER_ID = model.ItemRequest.SENDER_ID,
                THREAD_ID = model.ItemRequest.THREAD_ID
            };
            if (PrivateChatHistoryCollection == null) PrivateChatHistoryCollection = new ObservableRangeCollection<ChatHistoryItemDTO>();
            PrivateChatHistoryCollection.Add(emptyDTO);
            string UploadedResult = @"";
            if (model.ItemRequest.MSG_TYPE == GlobalConstants.CHAT_HISTORY_ITEM_AUDIO)
            {
                var currentIndex = AudioFileToBeSent.Count - 1;
                UploadedResult = await DataStore.UploadAudioToServer(model.FileName, model.FilebArray, emptyDTO);
            }
            model.ItemRequest.MSG_CONTENT = UploadedResult;
            PrivateChatHistoryCollection.Remove(emptyDTO);
            SendPrivateMessage(model.ItemRequest);
        }

        async Task ExecuteSendPrivateMessageComand(AddPrivateChatHistoryItemRequest model)
        {
            if (IsBusy) return;
            if (model.MSG_TYPE == GlobalConstants.CHAT_HISTORY_ITEM_IMAGE && PhotoFileToBeSent.Count == 0) return;
            if (model.MSG_TYPE == GlobalConstants.CHAT_HISTORY_ITEM_VIDEO && PhotoFileToBeSent.Count == 0) return;
            if (model.MSG_TYPE == GlobalConstants.CHAT_HISTORY_ITEM_PDF && BinaryFileToBeSent.Count == 0) return;

            if(model.MSG_TYPE == GlobalConstants.CHAT_HISTORY_ITEM_TEXT)
            {
                SendPrivateMessage(model);
            }
            else
            {
                var emptyDTO = new ChatHistoryItemDTO()
                {
                    ID = 0,
                    TYPE = model.MSG_TYPE, 
                    CONTENT = "", 
                    CREATED_AT = DateConverter.GetUnixTimeSpanFromDate(DateTime.Now), 
                    READ_STATUS = false, 
                    IS_FAVORITE = false,
                    SENDER_ID = model.SENDER_ID, 
                    THREAD_ID = model.THREAD_ID 
                };
                if (PrivateChatHistoryCollection == null) PrivateChatHistoryCollection = new ObservableRangeCollection<ChatHistoryItemDTO>();
                PrivateChatHistoryCollection.Add(emptyDTO);
                string UploadedResult = @"";
                if (model.MSG_TYPE != GlobalConstants.CHAT_HISTORY_ITEM_PDF)
                {
                    var currentIndex = PhotoFileToBeSent.Count - 1;
                    UploadedResult = await DataStore.UploadImageToServer(PhotoFileToBeSent[currentIndex], emptyDTO);
                }
                else
                {
                    var currentIndex = BinaryFileToBeSent.Count - 1;
                    UploadedResult = await DataStore.UploadImageToServer(BinaryFileToBeSent[currentIndex], emptyDTO);
                }
                PrivateChatHistoryCollection.Remove(emptyDTO);
                if(!string.IsNullOrEmpty(UploadedResult))
                {
                    model.MSG_CONTENT = UploadedResult;
                    SendPrivateMessage(model);    
                }
            }
        }

        bool SocketIsAlive()
        {
            if (SocketManagerDataStore.GetInstance().ChatHubProxy == null) return false;
            if (!SocketManagerDataStore.GetInstance().SocketStatus) return false;
            if (string.IsNullOrEmpty(SocketManagerDataStore.GetInstance().ConnectionId)) return false;
            return true;
        }

        private void SendPrivateMessage(AddPrivateChatHistoryItemRequest model)
        {
            SocketManagerDataStore.GetInstance().ChatHubProxy.Invoke("SendPrivateMessage", model);
        }

        void ExecuteOpenCreateChatEntrySocketCommand(ContactDTO model)
        {
            var requestModel = new OpenOrCreateChatEntryRequest()
            {
                ConnectionId = SocketManagerDataStore.GetInstance().ConnectionId,
                MYID = SocketManagerDataStore.GetInstance().UserId,
                OTHERID = model.USERID
            };
            if(!SocketIsAlive())
            {
                CurrentlyOpenDTO = null;
                return;
            }
            SocketManagerDataStore.GetInstance().ChatHubProxy.Invoke("OpenOrCreateEntry", requestModel);
        }

        #endregion

        void ExecuteRemoveUnreadCountCommand()
        {
            try
            {
                if(CurrentlyOpenDTO != null)
                {
                    CurrentlyOpenDTO.UnreadMessageCount = 0;
                    for (var i = 0; i < PrivateChatEntryCollection.Count(); i++)
                    {
                        if(PrivateChatEntryCollection[i].EntryID == CurrentlyOpenDTO.EntryID)
                        {
                            var TmpItem = PrivateChatEntryCollection[i];
                            TmpItem.UnreadMessageCount = 0;
                            PrivateChatEntryCollection.RemoveAt(i);
                            PrivateChatEntryCollection.Insert(i, TmpItem);
                        }
                    }
                    var notificationCountTmp = 0;
                    foreach (ChatEntryDTO model in PrivateChatEntryCollection)
                    {
                        notificationCountTmp += model.UnreadMessageCount;
                    }
                    NotificationCount = notificationCountTmp;
                }
            }
            catch(Exception e)
            {
                
            }
        }

        async Task ExecuteClearAllChatCommand(string token)
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                var clearResult = await DataStore.DeleteAllChatHistoryAsync(token);
                if (clearResult)
                {
                    CurrentlyOpenDTO = null;
                    if (PrivateChatHistoryCollection == null) PrivateChatEntryCollection = new ObservableRangeCollection<ChatEntryDTO>();
                    PrivateChatHistoryCollection.Clear();
                }
            }
            catch(Exception e)
            {
                
            }
            IsBusy = false;
        }

        async Task ExecuteLoadEntryHistory(GetPrivateChatEntryRequest value)
        {
            if (IsBusy)
                return;
            IsBusy = true;
            try
            {
                int notificationCountTmp = 0;
                var dtoList = await DataStore.GetChatHistoriesItems(value);

                if(dtoList != null)
                {
                    PrivateChatEntryCollection.Clear();
                    PrivateChatEntryCollection.AddRange(dtoList);
                }                    

                foreach(ChatEntryDTO model in PrivateChatEntryCollection)
                {
                    notificationCountTmp += model.UnreadMessageCount;
                }
                NotificationCount = notificationCountTmp;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
           
            IsBusy = false;
            IsFromUpdate = false;
          
        }

        void ExecuteChatSocketCommand(object userId)
        {
            SocketManagerDataStore.GetInstance().StartSocketConnection((int)userId);
            ChatHubProxy_NotifyOpenOrCreateThread();
            ChatHubProxy_NotifySendPrivateMessage();
            ChatHubProxy_NotifyNewReceivedPrivateMessage();
            ChatHubProxy_NotifyReadStatusMessage();
            ChatHubProxy_NotifyReceiveTypingStatusMessage();
            ChatHubProxy_NotifyReceiveUserOnline();
            ChatHubProxy_NotifyUserStatusChanged();
            ChatHubProxy_NotifyUserProfileChanged();
            ChatHubProxy_NotifySignOutChat();
            ChatHubProxy_NotifyUserSignInChat();
            ChatHubProxy_NotifyHistoryItemDeleted();
        }

        #region Socket Callback for user status, profile, signout, signIn
        void ChatHubProxy_NotifyUserSignInChat()
        {
            if (SocketManagerDataStore.GetInstance().ChatHubProxy == null) return;
            SocketManagerDataStore.GetInstance().ChatHubProxy.On<int, bool>("NotifyUserSignInChat", (UserId, OnlineStatus) =>
            {
                if (PrivateChatEntryCollection != null)
                {
                    for (var i = 0; i < PrivateChatEntryCollection.Count; i++)
                    {
                        if (PrivateChatEntryCollection[i].OtherUserId == UserId)
                        {
                            var Item = PrivateChatEntryCollection[i];
                            Item.OtherUserOnlineStatus = OnlineStatus;
                            PrivateChatEntryCollection.RemoveAt(i);
                            PrivateChatEntryCollection.Insert(i, Item);
                            break;
                        }
                    }
                }
            });
        }

        void ChatHubProxy_NotifySignOutChat()
        {
            if (SocketManagerDataStore.GetInstance().ChatHubProxy == null) return;
            SocketManagerDataStore.GetInstance().ChatHubProxy.On<int>("NotifySignOutChat", (UserId) =>
            {
                if (PrivateChatEntryCollection != null)
                {
                    for (var i = 0; i < PrivateChatEntryCollection.Count; i++)
                    {
                        if (PrivateChatEntryCollection[i].OtherUserId == UserId)
                        {
                            var Item = PrivateChatEntryCollection[i];
                            Item.OtherUserOnlineStatus = false;
                            PrivateChatEntryCollection.RemoveAt(i);
                            PrivateChatEntryCollection.Insert(i, Item);
                            break;
                        }
                    }
                }
            });
        }

        void ChatHubProxy_NotifyUserProfileChanged()
        {
            if (SocketManagerDataStore.GetInstance().ChatHubProxy == null) return;
            SocketManagerDataStore.GetInstance().ChatHubProxy.On<int, string, string, string>("NotifyUpdateUserProfile", (UserId, newPhoto, newName, newPhone) =>
            {
                if (PrivateChatEntryCollection != null)
                {
                    for (var i = 0; i < PrivateChatEntryCollection.Count; i++)
                    {
                        if (PrivateChatEntryCollection[i].OtherUserId == UserId)
                        {
                            var Item = PrivateChatEntryCollection[i];
                            Item.OtherUserPic = newPhoto;
                            Item.OtherUserName = newName;
                            PrivateChatEntryCollection.RemoveAt(i);
                            PrivateChatEntryCollection.Insert(i, Item);
                            break;
                        }
                    }
                }
            });
        }

        void ChatHubProxy_NotifyUserStatusChanged()
        {
            if (SocketManagerDataStore.GetInstance().ChatHubProxy == null) return;
            SocketManagerDataStore.GetInstance().ChatHubProxy.On<int, string>("UpdateUserStatusTitle", (UserId, newStatus) =>
            {
                if (PrivateChatEntryCollection != null)
                {
                    for (var i = 0; i < PrivateChatEntryCollection.Count; i++)
                    {
                        if(PrivateChatEntryCollection[i].OtherUserId == UserId)
                        {
                            var Item = PrivateChatEntryCollection[i];
                            Item.LastMessage = newStatus;
                            PrivateChatEntryCollection.RemoveAt(i);
                            PrivateChatEntryCollection.Insert(i, Item);
                            break;
                        }
                    }
                }
            }); 
        }

        #endregion

        #region NotifyCallbacks for Private Messages
        void ChatHubProxy_NotifyHistoryItemDeleted()
        {
            if (SocketManagerDataStore.GetInstance().ChatHubProxy == null) return;
            SocketManagerDataStore.GetInstance().ChatHubProxy.On<int>("PrivateChatHistoryDeleted", (historyId) =>
            {
                if(PrivateChatHistoryCollection != null)
                {
                    for (var index = 0; index < PrivateChatHistoryCollection.Count(); index++)
                    {
                        if(PrivateChatHistoryCollection[index].ID == historyId)
                        {
                            PrivateChatHistoryCollection.RemoveAt(index);
                            break;
                        }
                    }
                }
            });
        }

        void ChatHubProxy_NotifyReceiveUserOnline()
        {
            if (SocketManagerDataStore.GetInstance().ChatHubProxy == null) return;
            SocketManagerDataStore.GetInstance().ChatHubProxy.On<string, bool>("UserOnline", (UserId, onlineStatus) =>
            {
                var intUserId = Convert.ToInt32(UserId);
                for (var index = 0; index < PrivateChatEntryCollection.Count; index++)
                {
                    if(PrivateChatEntryCollection[index].OtherUserId == intUserId)
                    {
                        var TmpEntryDTO = PrivateChatEntryCollection[index];
                        TmpEntryDTO.OtherUserOnlineStatus = onlineStatus;
                        PrivateChatEntryCollection.RemoveAt(index);
                        PrivateChatEntryCollection.Insert(index, TmpEntryDTO);
                        break;
                    }
                }

                if(CurrentlyOpenDTO.OtherUserId == intUserId)
                {
                    var tmpDTO = CurrentlyOpenDTO;
                    tmpDTO.OtherUserOnlineStatus = onlineStatus;
                    CurrentlyOpenDTO = tmpDTO;
                }
            });  
        }

        void ChatHubProxy_NotifyReceiveTypingStatusMessage()
        {
            if (SocketManagerDataStore.GetInstance().ChatHubProxy == null) return;
            SocketManagerDataStore.GetInstance().ChatHubProxy.On<bool>("notifyOtherUserType", (typingStatus) =>
            {
                if (OtherTyping == typingStatus) return;
                OtherTyping = typingStatus;
            });    
        }

        private void ChatHubProxy_NotifyReadStatusMessage()
        {
            if (SocketManagerDataStore.GetInstance().ChatHubProxy == null) return;
            SocketManagerDataStore.GetInstance().ChatHubProxy.On<ChatHistoryItemDTO>("NotifyReadStatusMessage", (dto) =>
            {
                if (PrivateChatHistoryCollection != null)
                {
                    PrivateChatHistoryCollection.Where(u => u.ID == dto.ID).All(u => u.READ_STATUS = true);
                }
            });
        }

        private void ChatHubProxy_NotifySendPrivateMessage()
        {
            if (SocketManagerDataStore.GetInstance().ChatHubProxy == null) return;
          
            SocketManagerDataStore.GetInstance().ChatHubProxy.On<ChatHistoryItemDTO>("NotifySendPrivateMessage", (dto) =>
            {
                DonotSlideToEnd = false;
                PrivateChatHistoryCollection.Add(dto);
            });
        }

        private void ChatHubProxy_NotifyNewReceivedPrivateMessage()
        {
            if (SocketManagerDataStore.GetInstance().ChatHubProxy == null) return;
            SocketManagerDataStore.GetInstance().ChatHubProxy.On<ChatHistoryItemDTO>("NotifyNewReceivedPrivateMessage", (dto) => {
                PrivateChatHistoryCollection.Add(dto);
                SocketManagerDataStore.GetInstance().ChatHubProxy.Invoke("UpdateReadStatusPrivateMessage", dto);
            });
        }

        private void ChatHubProxy_NotifyOpenOrCreateThread()
        {
            if (SocketManagerDataStore.GetInstance().ChatHubProxy == null) return;
            SocketManagerDataStore.GetInstance().ChatHubProxy.On<ChatEntryDTO, string>("NotifyOpenOrCreateThread", (dto, connectionId) =>
            {
                SocketManagerDataStore.GetInstance().ConnectionId = connectionId;
                CheckExistDTOItem(dto);
                CurrentlyOpenDTO = null;
                CurrentlyOpenDTO = dto;
            });
        }
        #endregion

        void CheckExistDTOItem(ChatEntryDTO model)
        {
            var result = PrivateChatEntryCollection.FirstOrDefault(u => u.EntryID == model.EntryID);
            if(result == null )
            {
                PrivateChatEntryCollection.Add(model);
            }
        }

        async Task ExcuteUpdateItem(ChatEntryDTO item)
        {
            await Task.Delay(200);
        }

        async Task ExcuteClearAllSelection()
        {
            await Task.Delay(200);
        }
    }
}
