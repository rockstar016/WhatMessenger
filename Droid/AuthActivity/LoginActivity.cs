
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using API.Models.RequestModels;
using Plugin.Connectivity;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Rock.Utils;
using TaskManagerBLM.Droid.Sources.Utils;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Droid.Engine;
using WhatMessenger.Droid.Utils;
using WhatMessenger.Model.Auth;
using WhatMessenger.Model.Constants;
using WhatMessenger.ViewModel;
using WhatMessenger.ViewModel.ViewModels;

namespace WhatMessenger.Droid.AuthActivity
{
    [Activity(Label = "@string/app_name", Icon = "@mipmap/ic_launcher", RoundIcon = "@mipmap/ic_launcher_round",
              LaunchMode = LaunchMode.SingleInstance, Theme = "@style/AppTheme.NoActionBar",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
              ScreenOrientation = ScreenOrientation.Portrait, MainLauncher = true)]
    public class LoginActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_login;
        LoginViewModel ViewModel;      
        TextInputLayout editEmail, editPassword;
        Button LoginButton, RegisterButton, ForgotPassButton;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            ViewModel = new LoginViewModel();
            base.OnCreate(savedInstanceState);
            editEmail = FindViewById<TextInputLayout>(Resource.Id.editEmail);
            editPassword = FindViewById<TextInputLayout>(Resource.Id.editPassword);
            LoginButton = FindViewById<Button>(Resource.Id.btLogin);
            RegisterButton = FindViewById<Button>(Resource.Id.btRegister);
            ForgotPassButton = FindViewById<Button>(Resource.Id.btForgotPassword);
            LangSetting();
            LoginButton.Click += LoginButton_Click;
            RegisterButton.Click += RegisterButton_Click;
            ForgotPassButton.Click += ForgotPassButton_Click;
			CheckToken();
            //CheckAppPermission();
        }

        void LangSetting()
        {
            Dictionary<string, string> StringResource = base.MyApplication.CurrentLangSetting.GetStringResourceContents();
            FindViewById<TextView>(Resource.Id.txtHeader).Text = StringResource.GetValueOrDefault("HeaderTitleLogin");
            FindViewById<TextView>(Resource.Id.txtHeaderDescription).Text = StringResource.GetValueOrDefault("HeaderLoginDescription");
            editEmail.EditText.Hint = StringResource.GetValueOrDefault("EmailPlaceHolder");
            editPassword.EditText.Hint = StringResource.GetValueOrDefault("PasswordPlaceHolder");
            LoginButton.Text = StringResource.GetValueOrDefault("LoginTitle");
            RegisterButton.Text = StringResource.GetValueOrDefault("RegisterTitle");
            ForgotPassButton.Text = StringResource.GetValueOrDefault("ForgotPasswordTitle");
        }

		async Task CheckAppPermission()
        {
            try
            {
                var permissionResult = await CheckNecessaryPermissions();
                if (!permissionResult)
                {
                    await CheckNecessaryPermissions();
                }
                //CheckToken();
            }
            catch (Exception e)
            {
                
            }         
        }

        async Task<bool> CheckNecessaryPermissions()
        {
			var phoneStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Phone);
            if (phoneStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] {  Plugin.Permissions.Abstractions.Permission.Phone });
                phoneStatus = results[Plugin.Permissions.Abstractions.Permission.Phone];
            }
            return await Task.FromResult(phoneStatus == PermissionStatus.Granted);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnStart()
        {
            base.OnStart();
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        void CheckToken()
        {
            var token = PreferenceUtils.readString(this, PreferenceUtils.TOKEN);    
            if(!string.IsNullOrEmpty(token))
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    var model = new GetContactRequest() { TOKEN = token };
                    ViewModel.LoginWithTokenCommand.Execute(model);
                }
                else
                {
                    DialogUtils.ShowOKDialog(this, @"Warning", @"No Internet Connection");
                }
            }
        }

        protected override void OnStop()
        {
            base.OnStop();
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(LoginViewModel.IsBusy))
            {
                if(ViewModel.IsBusy)
                {
                    this.RunOnUiThread(()=>{ ShowLoadingDialog(@"Log in"); });
                }
                else
                {
                    this.RunOnUiThread(()=>{ HideLoadingDialog();});

                    if(ViewModel.LoginResultModel == null)
                    {
                        RunOnUiThread(() =>
                        {
                            DialogUtils.ShowOKDialog(this, @"Warning", @"Invalid authentication");
                        });

                        return;
                    }
                    else
                    {
                        if (ViewModel.LoginResultModel.RESULT)
                        {
                            this.MyApplication.Me = ViewModel.GetUserFromLoginResult();
                            if (EngineService.EngineInstance == null)
                            {
                                var startEngine = new Intent(this, typeof(EngineService));
                                StartService(startEngine);
                            }
                            PreferenceUtils.saveString(this, PreferenceUtils.LANG, MyApplication.Me.LANG == 1?"CH":"EN");
                            PreferenceUtils.saveString(this, PreferenceUtils.TOKEN, MyApplication.Me.TOKEN);
                            MyApplication.InitLangResource(MyApplication.Me.LANG == 1 ? "CH" : "EN");
                            GotoMainScreen();
                        }
                        else
                        {
                            if(string.Equals(ViewModel.LoginResultModel.MSG, GlobalConstants.RESPONSE_NO_USER_ERROR))
                            {
                                DialogUtils.ShowOKDialog(this, @"Warning", @"Invalid authentication");
                                return; 
                            }
                            else if(string.Equals(ViewModel.LoginResultModel.MSG, GlobalConstants.RESPONSE_BLOCK_ACCOUNT)) 
                            {
                                DialogUtils.ShowOKDialog(this, @"Warning", @"Account is not available.");
                                return; 
                            }
                            else if(string.Equals(ViewModel.LoginResultModel.MSG, GlobalConstants.RESPONSE_INVALID_IMEI))
                            {
                                DialogUtils.ShowOKDialog(this, @"Warning", @"You can't use this account on this device.");
                                return;
                            }
                            else
                            {
                                return; 
                            }
                        }
                    }
                }
            }
        }
       
		async void LoginButton_Click(object sender, EventArgs e)
        {
            if (StringCheckUtil.isEmpty(editEmail.EditText))
                return;

            if (StringCheckUtil.isEmpty(editPassword.EditText))
                return;

            if (CrossConnectivity.Current.IsConnected)
            {
				var permResult = await CheckNecessaryPermissions();
				if(!permResult)
				{
					DialogUtils.ShowOKDialog(this, @"Warning", @"Permission Error");
					return;
				}
                var Imei = PhoneIMEI.GetImei(this);
                ViewModel.LoginCommand.Execute(new LoginRequestModel() { EMAIL = editEmail.EditText.Text, PASSWORD = editPassword.EditText.Text, IMEI = Imei});    
            }
            else
            {
                DialogUtils.ShowOKDialog(this, @"Warning", @"No Internet Connection");
            }
        }

        void GotoMainScreen()
        {
            var MainContent = new Intent(this, typeof(MainActivity));
            StartActivity(MainContent);
            Finish();
        }

        void RegisterButton_Click(object sender, EventArgs e)
        {
            var RegisterContent = new Intent(this, typeof(RegisterActvity));
            StartActivity(RegisterContent);
            Finish();
        }

        void ForgotPassButton_Click(object sender, EventArgs e)
        {
            var mIntent = new Intent(this, typeof(ForgotPasswordActivity));
            StartActivity(mIntent);
        }

    }
}
