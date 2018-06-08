using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using API.Models.RequestModels;
using Microsoft.AspNet.SignalR.Client;
using MvvmHelpers;
using Newtonsoft.Json;
using Plugin.FilePicker.Abstractions;
using Plugin.Media.Abstractions;
using WhatMessenger.Model;
using WhatMessenger.Model.BaseModel;
using WhatMessenger.Model.Constants;
using WhatMessenger.Model.RequestModels;
using WhatMessenger.Services;
using WhatMessenger.ViewModel.Services;
using WhatMessenger.ViewModel.Utils;

namespace WhatMessenger.ViewModel
{
    public class GroupListViewModel : BaseViewModel
    {
        public GroupListDataStore DataStore;
        public ContactDataStore ContactStore;

        public ObservableCollection<GroupDTO> Items { get; set; }
        public ObservableCollection<ChoosableContact> ChoosableContactList { get; set; }
        public ObservableCollection<ChoosableContact> ChoosedContactList { get; set; }
        public ObservableRangeCollection<GroupHistoryItemDTO> GroupChatHistoryItemList { get; set; }

        GroupDTO currentlyOpenDTO;
        public GroupDTO CurrentlyOpenDTO { get => currentlyOpenDTO; set => SetProperty(ref currentlyOpenDTO, value); }

        public int MY_USER_ID;

        public ICommand LoadAllGroupListCommand { get; set; }
        public ICommand LoadAllContactListCommand { get; set; }
        public ICommand ChooseContactItemCommand { get; set; }
        public ICommand RemoveSelectedContactItemCommand { get; set; }
        public ICommand AddGroupItemCommand { get; set; }
        public ICommand ChangeCurrentGroupNameCommand { get; set; }
        public ICommand ChangeCurrentGroupAvatarCommand { get; set; }
        public ICommand ChangeCurrentGroupMemberCommand { get; set; }
        public ICommand CloseCurrentGroupCommand { get; set; }
        public ICommand LeaveCurrentGroupMemberCommand { get; set; }
        public ICommand GroupSocketStartCommand { get; set; }
        public ICommand LoadGroupMessageHistoryCommand { get; set; }
        public ICommand SendGroupMessageCommand { get; set; }
        public ICommand SendGroupMessageUploadFileCommand { get; set; }
        public ICommand ClearAllMessageHistoryCommand { get; set; }
        public ICommand StopUploadingOperationCommand { get; set; }
        public ICommand SendVoiceMessageCommand { get; set; }
        public ICommand ChoosedContactWithCurrentGroupDTOCommand { get; set; }
        public ICommand DeleteChatHistoryItemCommand { get; set; }

        public MediaFile GroupAvatar { get; set; }
        public List<MediaFile> PhotoFileToBeSent { get; set; }
        public List<FileData> BinaryFileToBeSent { get; set; }
        public List<string> AudioFileToBeSent { get; set; }


        public bool DonotSlideToEnd = false;
        public bool AlreadyConnected = false;
        public GroupListViewModel()
        {
            Title = "GroupListItem";
            DataStore = new GroupListDataStore();
            ContactStore = new ContactDataStore();
            PhotoFileToBeSent = new List<MediaFile>();
            BinaryFileToBeSent = new List<FileData>();
            AudioFileToBeSent = new List<string>();

            Items = new ObservableCollection<GroupDTO>();
            ChoosableContactList = new ObservableCollection<ChoosableContact>();
            ChoosedContactList = new ObservableCollection<ChoosableContact>();
            GroupChatHistoryItemList = new ObservableRangeCollection<GroupHistoryItemDTO>();

            LoadAllGroupListCommand = new Command<string>(async (token) => await ExecuteLoadAllGroupListCommand(token));
            ChangeCurrentGroupNameCommand = new Command<ContactAddRequest>(async (model) => await ExecuteCurrentGroupNameUpdate(model));
            ChangeCurrentGroupAvatarCommand = new Command<string>(async (token) => await ExecuteCurrentGroupAvatarUpdate(token));
            ChangeCurrentGroupMemberCommand = new Command<GetProfileRequest>(async (model) => await ExecuteCurrentGroupMemberUpdate(model));
            LeaveCurrentGroupMemberCommand = new Command<GetProfileRequest>(async (model) => await ExecuteLeaveCurrentGroupMember(model));
            CloseCurrentGroupCommand = new Command<string>(async (token) => await ExecuteCloseCurrentGroupCommand(token));
            ChoosedContactWithCurrentGroupDTOCommand = new Command(async () => await ExecuteChoosedContactWithCurrentGroupDTOCommand());
            LoadAllContactListCommand = new Command<string>(async (token) => await ExecuteLoadAllContactList(token));
            ChooseContactItemCommand = new Command<ChoosableContact>(async (value) => await ExecuteChooseContactItem(value));
            RemoveSelectedContactItemCommand = new Command<ChoosableContact>(async (value) => await ExecuteRemoveSelectedContactItem(value));
            AddGroupItemCommand = new Command<GroupAddRequest>(async (value) => await ExecuteAddGroupItemCommand(value));
            GroupSocketStartCommand = new Command<object>((value) => ExecuteChatSocketCommand(value));
            LoadGroupMessageHistoryCommand = new Command<LoadChatHistoryCollectionRequest>(async (value) => await ExecuteLoadGetPastMessageCommand(value));
            SendGroupMessageCommand = new Command<AddGroupChatHistoryItemRequest>(async (value) => await ExecuteSendGroupMessageCommand(value));
            SendGroupMessageUploadFileCommand = new Command<GroupHistoryItemDTO>((model) => ExecuteSendGroupMessageUploadFile(model));
            ClearAllMessageHistoryCommand = new Command<string>(async (token) => await ExecuteClearAllMessageHistoryCommand(token));
            StopUploadingOperationCommand = new Command<GroupHistoryItemDTO>(async (model) => await ExecuteStopUploadingOperationCommand(model));
            SendVoiceMessageCommand = new Command<AddGroupVoiceMessageItemRequest>((model) => ExecuteSendGroupVoiceMessageComand(model));
            DeleteChatHistoryItemCommand = new Command<Int32>((indexValue) => ExecuteRemoveGroupChatHistoryItem(indexValue));

        }
        void ExecuteRemoveGroupChatHistoryItem(Int32 indexOfValue)
        {
            if (GroupChatHistoryItemList == null) return;
            if (GroupChatHistoryItemList.Count() == 0) return;
            if (GroupChatHistoryItemList.Count() < indexOfValue) return;
            if (!CheckSocketStatus()) return;
            SocketManagerDataStore.GetInstance().ChatHubProxy.Invoke("RemoveGroupChatHistoryItem", GroupChatHistoryItemList[indexOfValue].ID, CurrentlyOpenDTO);
        }
        async Task ExecuteCurrentGroupAvatarUpdate(string token)
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var result = await DataStore.UploadImageToServer(GroupAvatar, token, Convert.ToString(CurrentlyOpenDTO.GROUP_ID));
                if (result != null)
                {
                    var tmpValue = CurrentlyOpenDTO;
                    tmpValue.GROUP_AVATAR = result;
                    CurrentlyOpenDTO = null;
                    CurrentlyOpenDTO = tmpValue;
                    if (CheckSocketStatus())
                    {
                        SocketManagerDataStore.GetInstance().ChatHubProxy.Invoke("UpdateGroupItem", CurrentlyOpenDTO);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task ExecuteCurrentGroupNameUpdate(ContactAddRequest model)
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var result = await DataStore.ChangeGroupName(model.TOKEN, model.MY_ID, model.OTHER_ID);
                if (result)
                {
                    var tmpValue = CurrentlyOpenDTO;
                    tmpValue.GROUP_NAME = model.OTHER_ID;
                    CurrentlyOpenDTO = null;
                    CurrentlyOpenDTO = tmpValue;
                    if (CheckSocketStatus())
                    {
                        SocketManagerDataStore.GetInstance().ChatHubProxy.Invoke("UpdateGroupItem", CurrentlyOpenDTO);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task ExecuteCurrentGroupMemberUpdate(GetProfileRequest model)
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                MY_USER_ID = Convert.ToInt32(model.USERID);

                var choosedList = new List<int>();
                foreach (var temp in ChoosedContactList)
                {
                    choosedList.Add(temp.ContactDTO.USERID);
                }
                choosedList.Add(MY_USER_ID);
                var strChoosedList = JsonConvert.SerializeObject(choosedList);
                var result = await DataStore.ChangeGroupMember(model.TOKEN, Convert.ToString(CurrentlyOpenDTO.GROUP_ID), strChoosedList);
                if (result != null)
                {
                    CurrentlyOpenDTO = null;
                    CurrentlyOpenDTO = result;
                    if (CheckSocketStatus())
                    {
                        SocketManagerDataStore.GetInstance().ChatHubProxy.Invoke("UpdateGroupItem", CurrentlyOpenDTO);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task ExecuteCloseCurrentGroupCommand(string token)
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var result = await DataStore.CloseGroup(token, Convert.ToString(CurrentlyOpenDTO.GROUP_ID));
                if (result)
                {
                    if (CheckSocketStatus())
                    {
                        SocketManagerDataStore.GetInstance().ChatHubProxy.Invoke("CloseGroupItem", CurrentlyOpenDTO.GROUP_ID);
                    }
                    CurrentlyOpenDTO = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
        /* *
         * TOKEN, USER ID
         * */
        async Task ExecuteLeaveCurrentGroupMember(GetProfileRequest model)
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                MY_USER_ID = Convert.ToInt32(model.USERID);
                var choosedList = new List<int>();
                foreach (var temp in CurrentlyOpenDTO.MEMBER_LIST)
                {
                    if(MY_USER_ID != temp.USERID) choosedList.Add(temp.USERID);
                }

                var strChoosedList = JsonConvert.SerializeObject(choosedList);
                var result = await DataStore.ChangeGroupMember(model.TOKEN, Convert.ToString(CurrentlyOpenDTO.GROUP_ID), strChoosedList);
                if (result != null)
                {
                    CurrentlyOpenDTO = null;
                    if (CheckSocketStatus())
                    {
                        SocketManagerDataStore.GetInstance().ChatHubProxy.Invoke("UpdateGroupItem", result);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task ExecuteStopUploadingOperationCommand(GroupHistoryItemDTO DTO)
        {
            var cancelResult = await DataStore.StopUploading(DTO);
            if (cancelResult)
            {
                GroupChatHistoryItemList.Remove(DTO);
            }
        }

        #region Socket managment region
        async void ExecuteSendGroupVoiceMessageComand(AddGroupVoiceMessageItemRequest model)
        {
            if (!CheckSocketStatus()) return;
            if (model.ItemRequest.MSG_TYPE == GlobalConstants.CHAT_HISTORY_ITEM_AUDIO && AudioFileToBeSent.Count == 0) return;

            var emptyDTO = new GroupHistoryItemDTO()
            {
                ID = 0,
                TYPE = model.ItemRequest.MSG_TYPE,
                CONTENT = "",
                CREATED_AT = DateConverter.GetUnixTimeSpanFromDate(DateTime.Now),
                READ_STATUS = false,
                SENDER_ID = model.ItemRequest.SENDER_ID,
                SENDER_NAME = model.ItemRequest.SENDER_NAME,
                THREAD_ID = model.ItemRequest.GROUP_ID
            };
            if (GroupChatHistoryItemList == null) GroupChatHistoryItemList = new ObservableRangeCollection<GroupHistoryItemDTO>();
            GroupChatHistoryItemList.Add(emptyDTO);
            string UploadedResult = @"";
            if (model.ItemRequest.MSG_TYPE == GlobalConstants.CHAT_HISTORY_ITEM_AUDIO)
            {
                var currentIndex = AudioFileToBeSent.Count - 1;
                UploadedResult = await DataStore.UploadAudioToServer(model.FileName, model.FilebArray, emptyDTO);
            }
            GroupChatHistoryItemList.Remove(emptyDTO);
            if (!string.IsNullOrEmpty(UploadedResult))
            {
                model.ItemRequest.MSG_CONTENT = UploadedResult;
                SendGroupMessage(model.ItemRequest);
            }
        }

        async Task ExecuteSendGroupMessageCommand(AddGroupChatHistoryItemRequest model)
        {
            if (IsBusy) return;
            if (model.MSG_TYPE == GlobalConstants.CHAT_HISTORY_ITEM_IMAGE && PhotoFileToBeSent.Count == 0) return;
            if (model.MSG_TYPE == GlobalConstants.CHAT_HISTORY_ITEM_VIDEO && PhotoFileToBeSent.Count == 0) return;
            if (model.MSG_TYPE == GlobalConstants.CHAT_HISTORY_ITEM_PDF && BinaryFileToBeSent.Count == 0) return;
            if (model.MSG_TYPE == GlobalConstants.CHAT_HISTORY_ITEM_TEXT)
            {
                SendGroupMessage(model);
            }
            else
            {
                var emptyDTO = new GroupHistoryItemDTO()
                {
                    ID = 0,
                    TYPE = model.MSG_TYPE,
                    CONTENT = "",
                    CREATED_AT = DateConverter.GetUnixTimeSpanFromDate(DateTime.Now),
                    READ_STATUS = false,
                    SENDER_ID = model.SENDER_ID,
                    THREAD_ID = model.GROUP_ID
                };
                if (GroupChatHistoryItemList == null) GroupChatHistoryItemList = new ObservableRangeCollection<GroupHistoryItemDTO>();
                GroupChatHistoryItemList.Add(emptyDTO);
                string UploadedResult = @"";
                if (model.MSG_TYPE != GlobalConstants.CHAT_HISTORY_ITEM_PDF)
                {
                    var currentIndex = PhotoFileToBeSent.Count - 1;
                    UploadedResult = await DataStore.UploadImageToServer(PhotoFileToBeSent[currentIndex], emptyDTO);
                }
                else
                {
                    var currentIndex = PhotoFileToBeSent.Count - 1;
                    UploadedResult = await DataStore.UploadImageToServer(BinaryFileToBeSent[currentIndex], emptyDTO);
                }
                GroupChatHistoryItemList.Remove(emptyDTO);
                if(!string.IsNullOrEmpty(UploadedResult))
                {
                    model.MSG_CONTENT = UploadedResult;
                    SendGroupMessage(model);    
                }
            }
        }

        private void SendGroupMessage(AddGroupChatHistoryItemRequest model)
        {
            SocketManagerDataStore.GetInstance().ChatHubProxy.Invoke("SendGroupMessage", model);
        }

        async Task ExecuteLoadGetPastMessageCommand(LoadChatHistoryCollectionRequest model)
        {
            if (IsBusy)
            {
                IsBusy = false;
                return;
            }
            IsBusy = true;
            var result = await DataStore.GetLoadGroupChatHistoryCollection(model);
            if (result != null)
            {
                CanLoadMore = result.Count == 100;
                if (GroupChatHistoryItemList.Count == 0)
                {
                    DonotSlideToEnd = false;
                    GroupChatHistoryItemList.AddRange(result);
                }
                else
                {
                    DonotSlideToEnd = true;
                    foreach (var historyItem in result)
                    {
                        GroupChatHistoryItemList.Insert(0, historyItem);
                    }
                }
            }
            IsBusy = false;
        }

        void ExecuteSendGroupMessageUploadFile(GroupHistoryItemDTO model)
        {
            if (!CheckSocketStatus()) return;
            SocketManagerDataStore.GetInstance().ChatHubProxy.Invoke("SendGroupMessageUploadFile", model);
        }

        void ExecuteChatSocketCommand(object userId)
        {
            //if (SocketManagerDataStore.GetInstance().ChatHubProxy == null) return;
            MY_USER_ID = (int)userId;
            SocketManagerDataStore.GetInstance().StartSocketConnection((int)userId);
            if (!AlreadyConnected)
            {
                ChatHubProxy_NotifyCreateNewGroup();
                ChatHubProxy_NotifySendGroupMessage();
                ChatHubProxy_NotifyNewReceivedGroupMessage();
                //ChatHubProxy_NotifyLoadPastWholeMessages();
                ChatHubProxy_NotifyUploadFinishMessage();
                ChatHubProxy_NotifyGroupUpdate();
                ChatHubProxy_NotifyGroupClose();
                ChatHubProxy_NotifyHistoryItemDeleted();
                AlreadyConnected = true;
            }
        }

        void ChatHubProxy_NotifyHistoryItemDeleted()
        {
            if (SocketManagerDataStore.GetInstance().ChatHubProxy == null) return;
            SocketManagerDataStore.GetInstance().ChatHubProxy.On<int>("GroupChatHistoryDeleted", (historyId) =>
            {
                if (GroupChatHistoryItemList != null)
                {
                    for (var index = 0; index < GroupChatHistoryItemList.Count(); index++)
                    {
                        if (GroupChatHistoryItemList[index].ID == historyId)
                        {
                            GroupChatHistoryItemList.RemoveAt(index);
                            break;
                        }
                    }
                }
            });
        }

        void ChatHubProxy_NotifyGroupClose()
        {
            if (SocketManagerDataStore.GetInstance().ChatHubProxy == null) return;
            SocketManagerDataStore.GetInstance().ChatHubProxy.On<int>("NotifyGroupClose", (dtoId) =>
            {
                if (Items != null)
                {
                    if (CurrentlyOpenDTO != null && CurrentlyOpenDTO.GROUP_ID == dtoId) CurrentlyOpenDTO = null;
                    for (var index = 0; index < Items.Count(); index++)
                    {
                        if(Items[index].GROUP_ID == dtoId)
                        {
                            Items.RemoveAt(index);
                            break;
                        }
                    }
                }
            });
        }

        void ChatHubProxy_NotifyGroupUpdate()
        {
            if (SocketManagerDataStore.GetInstance().ChatHubProxy == null) return;
            SocketManagerDataStore.GetInstance().ChatHubProxy.On<GroupDTO>("NotifyGroupUpdate", (dto) =>
            {
                if (Items != null)
                {
                    var existingItem = Items.FirstOrDefault<GroupDTO>(u => u.GROUP_ID == dto.GROUP_ID);
                    var containMe = dto.MEMBER_LIST.FirstOrDefault<UserDTO>(user => user.USERID == MY_USER_ID);

                    if (existingItem == null)
                    {
                        if (containMe != null)
                        {
                            Items.Add(dto);
                        }
                    }
                    else
                    {
                        for (var i = 0; i < Items.Count; i++)
                        {
                            if (Items[i].GROUP_ID == dto.GROUP_ID)
                            {
                                Items.RemoveAt(i);
                                if(containMe != null) 
                                {
                                    Items.Insert(i, dto); 
                                    if (CurrentlyOpenDTO.GROUP_ID == dto.GROUP_ID)
                                    {
                                        CurrentlyOpenDTO = dto;
                                    }
                                }
                                else
                                {
                                    if (CurrentlyOpenDTO.GROUP_ID == dto.GROUP_ID)
                                    {
                                        CurrentlyOpenDTO = null;
                                    }
                                }
                                break;
                            }
                        }    
                    }
                }
            });
        }

        void ChatHubProxy_NotifyUploadFinishMessage()
        {
            if (SocketManagerDataStore.GetInstance().ChatHubProxy == null) return;
            SocketManagerDataStore.GetInstance().ChatHubProxy.On<GroupHistoryItemDTO>("NotifyGroupUploadFinishMessage", (dto) =>
            {
                if (GroupChatHistoryItemList != null)
                {
                    for (var i = 0; i < GroupChatHistoryItemList.Count; i++)
                    {
                        if (GroupChatHistoryItemList[i].ID == dto.ID)
                        {
                            GroupChatHistoryItemList.RemoveAt(i);
                            GroupChatHistoryItemList.Insert(i, dto);
                            break;
                        }
                    }
                }
            });
        }

        private void ChatHubProxy_NotifySendGroupMessage()
        {
            if (SocketManagerDataStore.GetInstance().ChatHubProxy == null) return;
            SocketManagerDataStore.GetInstance().ChatHubProxy.On<GroupHistoryItemDTO>("NotifySendGroupMessage", (dto) =>
            {
                GroupChatHistoryItemList.Add(dto);
            });
        }

        private void ChatHubProxy_NotifyNewReceivedGroupMessage()
        {
            if (SocketManagerDataStore.GetInstance().ChatHubProxy == null) return;
            SocketManagerDataStore.GetInstance().ChatHubProxy.On<GroupHistoryItemDTO>("NotifyNewReceivedGroupMessage", (dto) => {
                GroupChatHistoryItemList.Add(dto);
            });
        }

        bool CheckSocketStatus()
        {
            if (SocketManagerDataStore.GetInstance().ChatHubProxy == null) return false;
            if (!SocketManagerDataStore.GetInstance().SocketStatus) return false;
            if (string.IsNullOrEmpty(SocketManagerDataStore.GetInstance().ConnectionId.Trim())) return false;
            return true;
        }

        //void ChatHubProxy_NotifyLoadPastWholeMessages()
        //{
        //    if (SocketManagerDataStore.GetInstance().ChatHubProxy == null) return;
        //    SocketManagerDataStore.GetInstance().ChatHubProxy.On<IList<GroupHistoryItemDTO>>("NotifyGroupLoadAllPastHistory", (dtoList) =>
        //    {
        //        if (GroupChatHistoryItemList == null) GroupChatHistoryItemList = new ObservableRangeCollection<GroupHistoryItemDTO>();
        //        GroupChatHistoryItemList.Clear();
        //        GroupChatHistoryItemList.AddRange(dtoList);
        //    });
        //}

        GroupDTO GetExistingDTO(GroupDTO dto)
        {
            foreach (var existingItem in Items)
            {
                if (existingItem.GROUP_ID == dto.GROUP_ID)
                {
                    return existingItem;        
                }
            }
            return null;
        }

        void ChatHubProxy_NotifyCreateNewGroup()
        {
            if (SocketManagerDataStore.GetInstance().ChatHubProxy == null) return;
            SocketManagerDataStore.GetInstance().ChatHubProxy.On<GroupDTO>("notifyCreateNewGroup", (dto) =>
            {
                if (Items == null) Items = new ObservableCollection<GroupDTO>();
                Items.Add(dto);
            });
        }

        #endregion

        #region Add Group 
        async Task ExecuteClearAllMessageHistoryCommand(string token)
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var clearResult = await DataStore.ClearChatHistories(token);
                if(clearResult)
                {
                    if (GroupChatHistoryItemList == null) GroupChatHistoryItemList = new ObservableRangeCollection<GroupHistoryItemDTO>();
                    GroupChatHistoryItemList.Clear();
                }
            }
            catch (Exception e)
            {

            }
            IsBusy = false;
        }

        async Task ExecuteAddGroupItemCommand(GroupAddRequest model)
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var choosedList = new List<int>();
                foreach(var temp in ChoosedContactList)
                {
                    choosedList.Add(temp.ContactDTO.USERID);
                }
                choosedList.Add(model.MY_USER_ID);
                var AddResult = await DataStore.AddItemAsync(model.TOKEN, choosedList, GroupAvatar, model.GROUP_NAME, Convert.ToString(model.MY_USER_ID), model.OWNER_NAME);
                if(AddResult != null && AddResult.RESULT)
                {
                    var groupDTO = JsonConvert.DeserializeObject<GroupDTO>(AddResult.MSG);
                    Items.Add(groupDTO);
                    GroupAvatar = null;
                    ChoosedContactList.Clear();
                    ChoosableContactList.Clear();

                    if(SocketManagerDataStore.GetInstance().ChatHubProxy != null)
                    {
                        SocketManagerDataStore.GetInstance().ChatHubProxy.Invoke("NewGroupCreated", groupDTO);
                    }
                }
            }
            catch(Exception e)
            {
                
            }

            IsBusy = false;

        }

        async Task ExecuteLoadAllContactList(string token)
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                ChoosableContactList.Clear();
                var MyContactList = await ContactStore.GetMyContactAsync(token);
                foreach (var item in MyContactList)
                {
                    ChoosableContactList.Add(new ChoosableContact() { Choose = false, ContactDTO = item });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            IsBusy = false;

        }
        #endregion

        #region Add, Remove Choosable Contact List
        async Task ExecuteChoosedContactWithCurrentGroupDTOCommand()
        {
            if (ChoosableContactList.Count() == 0) return;
            if (ChoosedContactList == null) ChoosedContactList = new ObservableCollection<ChoosableContact>();
            ChoosedContactList.Clear();
            foreach(var member in CurrentlyOpenDTO.MEMBER_LIST)
            {
                var existingChoosableItem  = ChoosableContactList.FirstOrDefault(u => u.ContactDTO.USERID == member.USERID);
                if(existingChoosableItem != null)
                {
                    ChoosableContactList.FirstOrDefault(u => u.ContactDTO.USERID == member.USERID).Choose = true;
                    await AddContactToChoosedExecute(existingChoosableItem);
                }
            }
        }

        async Task ExecuteRemoveSelectedContactItem(ChoosableContact value)
        {
            for (int i = 0; i < ChoosableContactList.Count; i++)
            {
                if (ChoosableContactList[i].ContactDTO.CONTACT_ID == value.ContactDTO.CONTACT_ID)
                {
                    ChoosableContactList.RemoveAt(i);
                    value.Choose = false;
                    ChoosableContactList.Insert(i, value);
                    await RemoveContactFromChoosedExecute(value);
                    break;
                }
            }
        }

        async Task ExecuteChooseContactItem(ChoosableContact value)
        {
            if(value.Choose)
            {
                for (int i = 0; i < ChoosableContactList.Count; i++)
                {
                    if(ChoosableContactList[i].ContactDTO.CONTACT_ID == value.ContactDTO.CONTACT_ID)
                    {
                        ChoosableContactList.RemoveAt(i);
                        value.Choose = false;
                        ChoosableContactList.Insert(i, value);
                        await RemoveContactFromChoosedExecute(value);
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < ChoosableContactList.Count; i++)
                {
                    if (ChoosableContactList[i].ContactDTO.CONTACT_ID == value.ContactDTO.CONTACT_ID)
                    {
                        ChoosableContactList.RemoveAt(i);
                        value.Choose = true;
                        ChoosableContactList.Insert(i, value);
                        await AddContactToChoosedExecute(value);
                        break;
                    }
                }
            }
        }

        async Task RemoveContactFromChoosedExecute(ChoosableContact value)
        {
            await Task.Factory.StartNew(() => {
                for (int i = 0; i < ChoosedContactList.Count; i++)
                {
                    if (ChoosedContactList[i].ContactDTO.CONTACT_ID == value.ContactDTO.CONTACT_ID)
                    {
                        ChoosedContactList.RemoveAt(i);
                        break;
                    }
                }
            });
        }

        async Task AddContactToChoosedExecute(ChoosableContact value)
        {
            await Task.Factory.StartNew(() =>
            {
                if (ChoosedContactList == null) ChoosedContactList = new ObservableCollection<ChoosableContact>();
                ChoosedContactList.Add(value);
            });

        }

        #endregion
       

        async Task ExecuteLoadAllGroupListCommand(string token)
        {
            if (IsBusy)
                return;
            IsBusy = true;
            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsAsync(token);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            IsBusy = false;

        }

    }
}