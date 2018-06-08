using System;
using System.Threading.Tasks;
using System.Windows.Input;
using API.Models.RequestModels;
using API.Models.ResponseModels;
using MvvmHelpers;
using Newtonsoft.Json;
using WhatMessenger.Model.Auth;
using WhatMessenger.Model.BaseModel;
using WhatMessenger.Model.RequestModels;
using WhatMessenger.Services;
using WhatMessenger.ViewModel.Services;

namespace WhatMessenger.ViewModel.ViewModels
{
    public class LoginViewModel:BaseViewModel
    {
        LoginDataStore DataStore;
        public CommonResponse LoginResultModel { get; set;}
        public CommonResponse RegisterResultModel { get; set;}
        CommonResponse askPasswordResult;
        public CommonResponse AskPasswordResult { get => askPasswordResult; set => SetProperty(ref askPasswordResult, value); }
        CommonResponse changePasswordResult;
        public CommonResponse ChangePasswordResult { get => changePasswordResult; set => SetProperty(ref changePasswordResult, value); }

        public ICommand LoginCommand { get; set; }
        public ICommand LoginWithTokenCommand { get; set; }
        public ICommand RegisterCommand { get; set; }
        public ICommand OnlineCommand { get; set; }
        public ICommand AskChangePasswordCommand { get; set; }
        public ICommand ChangePasswordCommand { get; set; }
        public LoginViewModel()
        {
            DataStore = new LoginDataStore();
            LoginCommand = new Command<LoginRequestModel>(async (requestModel) => { await LoginHandler(requestModel); });
            RegisterCommand = new Command<RegisterRequest>(async (requestModel) => { await RegisterHandler(requestModel); });
            LoginWithTokenCommand = new Command<GetContactRequest>(async (requestModel) => { await ExecuteLoginWithTokenCommand(requestModel); });
            AskChangePasswordCommand = new Command<AskPasswordChangeRequest>(async (requestModel) => { await ExecuteAskPasswordChangeRequestCommand(requestModel); });
            ChangePasswordCommand = new Command<ChangePasswordRequest>(async (requestModel) => { await ExecuteChangePasswordCommand(requestModel); });
        }

        async Task ExecuteAskPasswordChangeRequestCommand(AskPasswordChangeRequest model)
        {
            if (IsBusy) return;
            IsBusy = true;
            AskPasswordResult = await DataStore.AskPasswordChangeCommand(model);
            IsBusy = false;
        }

        async Task ExecuteChangePasswordCommand(ChangePasswordRequest model)
        {
            if (IsBusy) return;
            IsBusy = true;
            ChangePasswordResult = await DataStore.PasswordChangeCommand(model);
            IsBusy = false;
        }

        async Task ExecuteLoginWithTokenCommand(GetContactRequest model)
        {
            if (IsBusy) return;
            IsBusy = true;
            LoginResultModel = await DataStore.LoginWithToken(model);
            IsBusy = false;
        }

        async Task LoginHandler(LoginRequestModel model)
        {
            if (IsBusy) return;
            IsBusy = true;
            LoginResultModel =  await DataStore.LoginServiceExecute(model);
            IsBusy = false;
        }

        async Task RegisterHandler(RegisterRequest model)
        {
            if (IsBusy) return;
            IsBusy = true;
            RegisterResultModel = await DataStore.RegisterServiceExecute(model);
            IsBusy = false;
        }

        public UserDTO GetUserFromLoginResult()
        {
            var me = JsonConvert.DeserializeObject<UserDTO>(LoginResultModel.MSG);
            return me;
        }
    }
}
