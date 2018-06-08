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
using WhatMessenger.Model;
using WhatMessenger.Model.BaseModel;
using WhatMessenger.Model.Constants;
using WhatMessenger.Model.RequestModels;
using WhatMessenger.Services;
using WhatMessenger.ViewModel.Services;

namespace WhatMessenger.ViewModel
{
    public class ContactListViewModel: BaseViewModel
    {
        ContactDataStore DataStore = new ContactDataStore();

        public ICommand LoadAllContactListItemCommand { get; set; }
        public ICommand LoadContactItemCommand { get; set; }
        public ICommand SetCurrentItemCommand { get; set; }
        public ICommand LoadAllCandiateListItemCommand { get; set; }
        public ICommand ClearCandidateListItemCommand { get; set; }
        public ICommand AddContactListItemCommand { get; set; }
        public ICommand SetMeOnlineStatusCommand { get; set; }
        public ICommand BlockContactCommand { get; set; }
        public ICommand UnblockContactCommand { get; set; }
        public ICommand StartChatSocketCommand { get; set; }

        public ObservableRangeCollection<ContactDTO> Items { get; set; }
        public ObservableCollection<ContactDTO> CandiateList { get; set; }
        private string addContactHappen;
        public string AddContactHappen
        {
            get
            {
                return addContactHappen;
            }
            set
            {
                SetProperty(ref addContactHappen, value);
            }
        }

        private ContactDTO currentOpenContactDTO;
        public ContactDTO CurrentOpenContactDTO
        {
            get
            {
                return currentOpenContactDTO;
            }
            set
            {
                SetProperty(ref currentOpenContactDTO, value);
            }
        }

        public ContactListViewModel()
        {
            Title = "ContactListItem";
            Items = new ObservableRangeCollection<ContactDTO>();
            CandiateList = new ObservableCollection<ContactDTO>();
            LoadAllContactListItemCommand = new Command<string>(async (token) => await ExecuteLoadContactList(token));
            LoadContactItemCommand = new Command<GetProfileRequest>(async (model) => await ExecuteLoadContactItemCommand(model));
            LoadAllCandiateListItemCommand = new Command<ContactCandidateRequest>(async (model) => await ExecuteLoadCandidateList(model));
            ClearCandidateListItemCommand = new Command(async () => await ExecuteClearCandidateList());
            SetCurrentItemCommand = new Command<Int32>((userId) => ExecuteSetCurrentItem(userId));
                                                
            AddContactListItemCommand = new Command<ContactAddRequest>(async (model) => await ExecuteAddContact(model));
            SetMeOnlineStatusCommand = new Command<OnlineStatusRequest>((value) => ExecuteMeOnlineStatusCommand(value));
            BlockContactCommand = new Command<GetProfileRequest>(async (value) => await ExecuteBlockContactCommand(value));
            UnblockContactCommand = new Command<GetProfileRequest>(async (value) => await ExecuteUnblockContactCommand(value));
            StartChatSocketCommand = new Command<object>((userId) => ExecuteChatSocketCommand(userId));
        }



        #region Socket Callback for user status, profile, signout
        void ExecuteChatSocketCommand(object userId)
        {
            //SocketManagerDataStore.GetInstance().StartSocketConnection((int)userId);
            ChatHubProxy_NotifyUserStatusChanged();
            ChatHubProxy_NotifyUserProfileChanged();
        }

        void ChatHubProxy_NotifyUserProfileChanged()
        {
            if (SocketManagerDataStore.GetInstance().ChatHubProxy == null) return;
            SocketManagerDataStore.GetInstance().ChatHubProxy.On<int, string, string, string>("NotifyUpdateContact", (UserId, newPhoto, newName, newPhone) =>
            {
                if (Items != null)
                {
                    for (var i = 0; i < Items.Count; i++)
                    {
                        if (Items[i].USERID == UserId)
                        {
                            var Item = Items[i];
                            Item.PHONE = newPhone;
                            Item.PIC = newPhoto;
                            Item.NAME = newName;
                            Items.RemoveAt(i);
                            Items.Insert(i, Item);
                            break;
                        }
                    }
                }
            });
        }

        void ChatHubProxy_NotifyUserStatusChanged()
        {
            if (SocketManagerDataStore.GetInstance().ChatHubProxy == null) return;
            SocketManagerDataStore.GetInstance().ChatHubProxy.On<int, string>("UpdateContactStatusTitle", (UserId, newStatus) =>
            {
                if (Items != null)
                {
                    for (var i = 0; i < Items.Count; i++)
                    {
                        if (Items[i].USERID == UserId)
                        {
                            var Item = Items[i];
                            Item.USER_STATUS_TITLE = newStatus;
                            Items.RemoveAt(i);
                            Items.Insert(i, Item);
                            break;
                        }
                    }
                }
            });
        }

        #endregion

        void ExecuteSetCurrentItem(Int32 UserId)
        {
            if (Items == null) Items = new ObservableRangeCollection<ContactDTO>();
            CurrentOpenContactDTO = Items.FirstOrDefault(u => u.USERID == UserId);
        }

        async Task ExecuteLoadContactItemCommand(GetProfileRequest model)
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                CurrentOpenContactDTO = await DataStore.GetContactDetailAsync(model);
            }
            catch(Exception exp)
            {
                
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task ExecuteUnblockContactCommand(GetProfileRequest model)
        {
            if (IsBusy)
                return;
            IsBusy = true;
            try
            {
                var blockResult = await DataStore.UnblockAccountAsync(model);
                if (blockResult)
                {
                    var contactIdToBlock = Convert.ToInt32(model.USERID);
                    foreach (var item in Items)
                    {
                        if (item.CONTACT_ID == contactIdToBlock)
                        {
                            var ItemTemp = item;
                            Items.Remove(item);
                            ItemTemp.IS_I_BLOCKED = false;
                            Items.Add(ItemTemp);
                        }
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


        async Task ExecuteBlockContactCommand(GetProfileRequest model)
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var blockResult = await DataStore.BlockAccountAsync(model);
                if(blockResult)
                {
                    var contactIdToBlock = Convert.ToInt32(model.USERID);
                    foreach(var item in Items)
                    {
                        if(item.CONTACT_ID == contactIdToBlock)
                        {
                            var ItemTemp = item;
                            Items.Remove(item);
                            ItemTemp.IS_I_BLOCKED = true;
                            Items.Add(ItemTemp);
                        }
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

        void ExecuteMeOnlineStatusCommand(OnlineStatusRequest model)
        {
            try
            {
                SocketManagerDataStore.GetInstance().ChatHubProxy.Invoke("SetOnlineStatus", model.UserId, model.IsOnline);    
            }
            catch (Exception e)
            {
                
            }
        }

        async Task<bool> ExecuteLoadContactList(string token)
        {
            if (IsBusy)
                return true;
            IsBusy = true;
            try
            {
                Items.Clear();
                var items = await DataStore.GetMyContactAsync(token);
                foreach(var item in items)
                {
                    Items.Add(item);
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
            return true;
        }

        async Task<bool> ExecuteAddContact(ContactAddRequest model)
        {
            if (IsBusy)
                return true;
            IsBusy = true;
            try
            {
                var item = await DataStore.AddContactAsync(model);
                if(item.RESULT)
                {
                    AddContactHappen = string.Empty;
                    var contactDTO = JsonConvert.DeserializeObject<ContactDTO>(item.MSG);
                    var listDTO = new List<ContactDTO>() { contactDTO };
                    if(Items == null ) Items = new ObservableRangeCollection<ContactDTO>();
                    Items.AddRange(listDTO, System.Collections.Specialized.NotifyCollectionChangedAction.Add);
                }
                else
                {
                    AddContactHappen = item.MSG;
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
            return true;
        }
        async Task<bool> ExecuteLoadCandidateList(ContactCandidateRequest model)
        {
            if (IsBusy)
                return true;
            IsBusy = true;
            try
            {
                CandiateList.Clear();
                var CandidateResultFromStore = await DataStore.GetCandidateAsync(model);
                if (CandidateResultFromStore == null)
                    CandidateResultFromStore = new List<ContactDTO>();
                foreach (var item in CandidateResultFromStore)
                {
                    CandiateList.Add(item);
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
            return true;
        }

        async Task<bool> ExecuteClearCandidateList()
        {
            if (IsBusy)
                return true;
            IsBusy = true;
            try
            {
                await Task.Factory.StartNew(() => {
                    CandiateList.Clear();    
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
            return true;
        }
    }
}
