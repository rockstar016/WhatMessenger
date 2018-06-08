using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using API.Models.RequestModels;
using API.Models.ResponseModels;
using MvvmHelpers;
using Newtonsoft.Json;
using Plugin.Media.Abstractions;
using WhatMessenger.Model;
using WhatMessenger.Model.BaseModel;
using WhatMessenger.Services;

namespace WhatMessenger.ViewModel
{
    public class ProfileViewModel : BaseViewModel
    {
        ProfileDataStore DataStore;
        private UserDTO me;
        public UserDTO ME { get => me; set => SetProperty(ref me, value); }

        public bool IsUpdatePhone;
        public ICommand CommandGetMyProfile { get; set; }
        public ICommand CommandCloseAccount { get; set; }
        public ICommand CommandSignOutAccount { get; set; }
        public ICommand CommandUpdatePhone { get; set; }
        public ICommand CommandUpdateName { get; set; }
        public ICommand CommandUpdatePhoto { get; set; }
        public ICommand CommandUpdatePassword { get; set; }
        public ICommand CommandUpdateLang { get; set; }
        public ICommand CommandUpdateStatusTitle { get; set; }
        public ICommand CommandUpdateProfileShareTo { get; set; }
        public ICommand CommandUpdateStatusShareTo { get; set; }


        public MediaFile NewProfilePhoto { get; set; }
        CommonResponse closeAccountResult;
        public CommonResponse CloseAccountResult
        {
            get
            {
                return closeAccountResult;
            }
            set
            {
                SetProperty(ref closeAccountResult, value);
            }
        }
        bool isSignOut;
        public bool IsSignOut
        {
            get => isSignOut;
            set{
                SetProperty(ref isSignOut, value);
            }
        }

        public ProfileViewModel()
        {
            DataStore = new ProfileDataStore();
            CommandGetMyProfile = new Command<GetProfileRequest>(async (model) => await CommandGetMyProfile_Execute(model));
            CommandCloseAccount = new Command<string>(async (token) => await CloseAccountCommand_Execute(token));
            CommandUpdateName = new Command<GetProfileRequest>(async (model) => await CommandUpdateName_Execute(model));
            CommandUpdatePhone = new Command<GetProfileRequest>(async (model) => await CommandUpdatePhone_Execute(model));
            CommandUpdatePhoto = new Command<string>(async (token) => await CommandUpdatePhoto_Execute(token));
            CommandUpdatePassword = new Command<ResetPasswordRequest>(async (model) => await CommandUpdatePassword_Execute(model));
            CommandUpdateLang = new Command<GetProfileRequest>(async (model) => await CommandUpdateLang_Execute(model));
            CommandUpdateStatusTitle = new Command<GetProfileRequest>(async (model) => await CommandUpdateStatusTitle_Execute(model));
            CommandUpdateProfileShareTo = new Command<GetProfileRequest>(async (model) => await CommandUpdateProfileShareTo_Execute(model));
            CommandUpdateStatusShareTo = new Command<GetProfileRequest>(async (model) => await CommandUpdateStatusShareTo_Execute(model));
            CommandSignOutAccount = new Command<string>(async (token) => await CommandSignOutAccount_Execute(token));
        }

        async Task CommandSignOutAccount_Execute(string Token)
        {
            if (IsBusy) return;
            IsSignOut = false;
            IsBusy = true;
            try
            {
                var SignOutResult = await DataStore.SignOutAccount(Token);
                if (SignOutResult != null && SignOutResult.RESULT)
                {
                    if (SocketManagerDataStore.GetInstance().SocketStatus)
                    {
                        SocketManagerDataStore.GetInstance().ChatHubProxy.Invoke("UserSignOut", Convert.ToInt32(SignOutResult.MSG));
                    }
                }
                IsSignOut = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task CommandUpdateStatusTitle_Execute(GetProfileRequest model)
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var UpdateStatusResult = await DataStore.UpdateUserStatusTitle(model);
                if (UpdateStatusResult != null && UpdateStatusResult.RESULT)
                {
                    var newMe = JsonConvert.DeserializeObject<UserDTO>(UpdateStatusResult.MSG);
                    ME = newMe;
                    if(SocketManagerDataStore.GetInstance().SocketStatus) 
                    {
                        SocketManagerDataStore.GetInstance().ChatHubProxy.Invoke("UserStatusChanged", newMe.USERID, newMe.SHOW_STATUS_TO);

                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task CommandUpdateProfileShareTo_Execute(GetProfileRequest model)
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var UpdateLangResult = await DataStore.UpdateUserProfileShareTo(model);
                if (UpdateLangResult != null && UpdateLangResult.RESULT)
                {
                    var newMe = JsonConvert.DeserializeObject<UserDTO>(UpdateLangResult.MSG);
                    ME = newMe;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
            finally
            {
                IsBusy = false;
            }   
        }

        async Task CommandUpdateStatusShareTo_Execute(GetProfileRequest model)
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var UpdateLangResult = await DataStore.UpdateUserStatusShareTo(model);
                if (UpdateLangResult != null && UpdateLangResult.RESULT)
                {
                    var newMe = JsonConvert.DeserializeObject<UserDTO>(UpdateLangResult.MSG);
                    ME = newMe;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task CommandUpdateLang_Execute(GetProfileRequest model)
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var UpdateLangResult = await DataStore.UpdateUserLang(model);
                if (UpdateLangResult != null && UpdateLangResult.RESULT)
                {
                    var newMe = JsonConvert.DeserializeObject<UserDTO>(UpdateLangResult.MSG);
                    ME = newMe;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task CommandUpdatePassword_Execute(ResetPasswordRequest model)
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var UpdatePasswordResult = await DataStore.UpdateUserPassword(model);
                if (UpdatePasswordResult != null && UpdatePasswordResult.RESULT)
                {
                    var newMe = JsonConvert.DeserializeObject<UserDTO>(UpdatePasswordResult.MSG);
                    ME = newMe;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
            finally
            {
                IsBusy = false;
            }    
        }

        async Task CommandUpdatePhoto_Execute(string token)
        {
            if (NewProfilePhoto == null)
                return;
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var UpdatePhotoResult = await DataStore.UpdateUserPhoto(token, NewProfilePhoto);
                if (UpdatePhotoResult != null && UpdatePhotoResult.RESULT)
                {
                    var newMe = JsonConvert.DeserializeObject<UserDTO>(UpdatePhotoResult.MSG);
                    ME = newMe;
                    if (SocketManagerDataStore.GetInstance().SocketStatus && ME.SHOW_STATUS_TO != 2)
                    {
                        SocketManagerDataStore.GetInstance().ChatHubProxy.Invoke("UserProfileChange", newMe.USERID, newMe.SHOW_PROFILE_TO);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task CommandUpdateName_Execute(GetProfileRequest model)
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var UpdateNameResult = await DataStore.UpdateUserName(model);
                if(UpdateNameResult != null && UpdateNameResult.RESULT)
                {
                    var newMe = JsonConvert.DeserializeObject<UserDTO>(UpdateNameResult.MSG);
                    ME = newMe;
                    if (SocketManagerDataStore.GetInstance().SocketStatus && ME.SHOW_STATUS_TO != 2)
                    {
                        SocketManagerDataStore.GetInstance().ChatHubProxy.Invoke("UserProfileChange", newMe.USERID, newMe.SHOW_PROFILE_TO);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task CommandUpdatePhone_Execute(GetProfileRequest model)
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var UpdatePhoneResult = await DataStore.UpdateUserPhone(model);
                if(UpdatePhoneResult != null && UpdatePhoneResult.RESULT)
                {
                    var newMe = JsonConvert.DeserializeObject<UserDTO>(UpdatePhoneResult.MSG);
                    ME = newMe;
                    if (SocketManagerDataStore.GetInstance().SocketStatus && ME.SHOW_STATUS_TO != 2)
                    {
                        SocketManagerDataStore.GetInstance().ChatHubProxy.Invoke("UserProfileChange", newMe.USERID, newMe.SHOW_PROFILE_TO);
                    }
                }
            }
            catch (Exception e)
            {
                
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task CloseAccountCommand_Execute(string token)
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var closeResult = await DataStore.CloseAccount(token);
                if (SocketManagerDataStore.GetInstance().SocketStatus && closeResult.RESULT)
                {
                    SocketManagerDataStore.GetInstance().ChatHubProxy.Invoke("UserAccountClosed", Convert.ToInt32(closeResult.MSG));
                }
                CloseAccountResult = closeResult;
            }
            catch(Exception e)
            {
                CloseAccountResult = null;
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task CommandGetMyProfile_Execute(GetProfileRequest model)
        {
            if (IsBusy)
                return;
            IsBusy = true;
            try
            {
                var ProfileGetRequestResult = await DataStore.GetProfile(model);
                if(ProfileGetRequestResult.RESULT)
                {
                    ME = JsonConvert.DeserializeObject<UserDTO>(ProfileGetRequestResult.MSG);
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
    }
}
