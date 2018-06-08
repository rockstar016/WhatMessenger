using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using Plugin.CurrentActivity;
using Plugin.Media;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Rock.Utils;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Droid.Engine;
using WhatMessenger.Model.Constants;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.Account
{
    [Activity(Label = "")]
    public class MyProfileEditActivity : BaseActivity
    {
        ProfileViewModel ThisViewModel;
        protected override int LayoutResource => Resource.Layout.activity_my_profile;
        ImageViewAsync imgMyProfile;
        FloatingActionButton fabChangePhoto;
        TextView txtUserName, txtUserNameIndicator;
        ImageButton btEditUserName;
        TextView txtUserPhone, txtUserPhoneIndicator;
        ImageButton btEditUserPhone;
        TextView txtUserEmail, txtUserEmailIndicator;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            ThisViewModel = EngineService.EngineInstance.ProfileViewModel;
            base.OnCreate(savedInstanceState);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Title = "";

            txtUserPhone = FindViewById<TextView>(Resource.Id.txtUserPhone);
            txtUserName = FindViewById<TextView>(Resource.Id.txtUserName);
            txtUserEmail = FindViewById<TextView>(Resource.Id.txtUserEmail);

            txtUserNameIndicator = FindViewById<TextView>(Resource.Id.txtUserNameIndicator);
            txtUserPhoneIndicator = FindViewById<TextView>(Resource.Id.txtUserPhoneIndicator);
            txtUserEmailIndicator = FindViewById<TextView>(Resource.Id.txtUserEmailIndicator);

            fabChangePhoto = FindViewById<FloatingActionButton>(Resource.Id.fabChangePhoto);
            imgMyProfile = FindViewById<ImageViewAsync>(Resource.Id.imgMyProfile);

            btEditUserPhone = FindViewById<ImageButton>(Resource.Id.btEditUserPhone);
            btEditUserName = FindViewById<ImageButton>(Resource.Id.btEditUserName);

            btEditUserName.Click += BtEditUserName_Click;
            btEditUserPhone.Click += BtEditUserPhone_Click;
            fabChangePhoto.Click += FabChangePhoto_Click;

            CrossCurrentActivity.Current.Activity = this;
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

        void FabChangePhoto_Click(object sender, EventArgs e)
        {
            
            var photoOptions = new string[] { @"From Gallery", @"Take Photo" };
            new Android.Support.V7.App.AlertDialog.Builder(this)
                           .SetTitle("Choose Options")
                           .SetItems(photoOptions, (sender1, e1) =>
                           {
                               if (e1.Which == 0)
                               {
                                   OpenGallery();
                               }
                               else
                               {
                                   OpenCamera();
                               }
                           })
                           .Create()
                           .Show();
        }

        async void OpenGallery()
        {
            var PermStorage = await CheckNecessaryPermissions(Permission.Storage);
            if (!PermStorage) return;

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsPickPhotoSupported)
            {
                DialogUtils.ShowOKDialog(this, @"Warning", @"No camera available.");
                return;
            }

            var ProfileImageFile = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small
            });

            if (ProfileImageFile != null)
            {
                ImageService.Instance
                            .LoadFile(ProfileImageFile.Path)
                            .ErrorPlaceholder("male_placeholder", FFImageLoading.Work.ImageSource.CompiledResource)
                            .LoadingPlaceholder("male_placeholder", FFImageLoading.Work.ImageSource.CompiledResource)
                            .Retry(3, 200)
                            .Into(imgMyProfile);
                ThisViewModel.NewProfilePhoto = ProfileImageFile;
                ThisViewModel.CommandUpdatePhoto.Execute(MyApplication.Me.TOKEN);
            }
        }

        async void OpenCamera()
        {
            //check permission here
            var CamPermission = await CheckNecessaryPermissions(Permission.Camera);
            var StoragePermission = await CheckNecessaryPermissions(Permission.Storage);
            if (!CamPermission || !StoragePermission)
                return;
            
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                DialogUtils.ShowOKDialog(this, "Warning", "No camera available.");
                return;
            }

            var ProfileImageFile = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Profile",
                Name = DateTime.Now.ToShortDateString() + ".jpg",
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small
            });

            if (ProfileImageFile != null)
            {
                ImageService.Instance
                            .LoadFile(ProfileImageFile.Path)
                            .ErrorPlaceholder("male_placeholder", FFImageLoading.Work.ImageSource.CompiledResource)
                            .LoadingPlaceholder("male_placeholder", FFImageLoading.Work.ImageSource.CompiledResource)
                            .Retry(3, 200)
                            .Into(imgMyProfile);
                ThisViewModel.NewProfilePhoto = ProfileImageFile;
                ThisViewModel.CommandUpdatePhoto.Execute(MyApplication.Me.TOKEN);
            }
        }

        void BtEditUserPhone_Click(object sender, EventArgs e)
        {
            ThisViewModel.IsUpdatePhone = true;
            var mIntent = new Intent(this, typeof(ProfileSubmitActivity));
            StartActivity(mIntent);
        }

        void BtEditUserName_Click(object sender, EventArgs e)
        {
            ThisViewModel.IsUpdatePhone = false;
            var mIntent = new Intent(this, typeof(ProfileSubmitActivity));
            StartActivity(mIntent);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if(item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
            }
            return true;
        }

        protected override void OnStart()
        {
            base.OnStart();
            ThisViewModel.PropertyChanged += ThisViewModel_PropertyChanged;
            if(ThisViewModel.ME == null)
            {
                ThisViewModel.ME = MyApplication.Me;
            }
            else
            {
                UpdateCurrentUI();    
            }

        }

        protected override void OnStop()
        {
            base.OnStop();
            ThisViewModel.PropertyChanged -= ThisViewModel_PropertyChanged;
        }

        void ThisViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(string.Equals(e.PropertyName, nameof(ProfileViewModel.IsBusy)))
            {
                if(ThisViewModel.IsBusy)
                {
                    ShowLoadingDialog("Loading");
                }
                else
                {
                    HideLoadingDialog();
                }
            }

            if(string.Equals(e.PropertyName, nameof(ProfileViewModel.ME)))
            {
                UpdateCurrentUI();
                MyApplication.Me = ThisViewModel.ME;
                ThisViewModel.NewProfilePhoto = null;
            }
        }

        void UpdateCurrentUI()
        {
            if(ThisViewModel.ME != null)
            {
                txtUserName.Text  = string.IsNullOrEmpty(ThisViewModel.ME.NAME.Trim())?@"No Name": ThisViewModel.ME.NAME.Trim();
                txtUserEmail.Text  = string.IsNullOrEmpty(ThisViewModel.ME.EMAIL.Trim())?@"No Email":ThisViewModel.ME.EMAIL.Trim();
                txtUserPhone.Text = string.IsNullOrEmpty(ThisViewModel.ME.PHONE.Trim())?@"No Phone Number":ThisViewModel.ME.PHONE.Trim();
                ImageService.Instance.LoadUrl(ServerURL.BaseURL + ThisViewModel.ME.PHOTO)
                            .ErrorPlaceholder("male_placeholder", FFImageLoading.Work.ImageSource.CompiledResource)
                            .LoadingPlaceholder("male_placeholder", FFImageLoading.Work.ImageSource.CompiledResource)
                            .Retry(3, 200)
                            .Into(imgMyProfile);
            }
        }
    }
}
