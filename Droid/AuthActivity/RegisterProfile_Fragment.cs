
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Refractored.Controls;
using Rock.Utils;
using Square.Picasso;
using WhatMessenger.Droid.Bases;

namespace WhatMessenger.Droid.AuthActivity
{
    public class RegisterProfile_Fragment : RegisterBaseFragment
    {
        public static RegisterProfile_Fragment GetInstance() => new RegisterProfile_Fragment { Arguments = new Bundle() };
        EditText txtName;
        CircleImageView imgProfile;
        MediaFile ProfileImageFile;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var rootView = inflater.Inflate(Resource.Layout.fragment_register_profile, container, false);
            var NextButton = rootView.FindViewById<Button>(Resource.Id.btNext);
            NextButton.Click += BtNext_Click;
            Dictionary<string, string> StringResource = ParentActivity.MyApplication.CurrentLangSetting.GetStringResourceContents();
            NextButton.Text = StringResource.GetValueOrDefault("Next");

            rootView.FindViewById<TextView>(Resource.Id.txtTitle).Text = StringResource.GetValueOrDefault("txtHeader");
            rootView.FindViewById<TextView>(Resource.Id.txtDescription).Text = StringResource.GetValueOrDefault("txtDescription");
            txtName = rootView.FindViewById<EditText>(Resource.Id.txtName);
            txtName.Hint = StringResource.GetValueOrDefault("txtName");
            imgProfile = rootView.FindViewById<CircleImageView>(Resource.Id.profile_image);
            imgProfile.Click += BtCamera_Click;
            InitViews();
            return rootView;
        }

        void BtCamera_Click(object sender, EventArgs e)
        {
            var photoOptions = new string[] { @"From Gallery", @"Take Photo" };
            new Android.Support.V7.App.AlertDialog.Builder(this.Context)
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
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsPickPhotoSupported)
            {
                DialogUtils.ShowOKDialog(this.Activity, @"Warning", @"No camera available.");
                return;
            }

            ProfileImageFile = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small
            });

            if (ProfileImageFile != null)
            {
                Picasso.With(this.ParentActivity)
                       .Load(new Java.IO.File(ProfileImageFile.Path))
                       .Error(Resource.Drawable.camera_snapshot)
                       .Into(imgProfile);
            }
        }

        async void OpenCamera()
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                DialogUtils.ShowOKDialog(this.Activity, "Warning", "No camera available.");
                return;
            }

            ProfileImageFile = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Profile",
                Name = DateTime.Now.ToShortDateString() + ".jpg",
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small
            });

            if (ProfileImageFile != null)
            {
                Picasso.With(this.ParentActivity)
                       .Load(new Java.IO.File(ProfileImageFile.Path))
                       .Error(Resource.Drawable.camera_snapshot)
                       .Into(imgProfile);
            }
        }

        void InitViews()
        {
            if(string.IsNullOrEmpty(ParentActivity.RegisterModel.FILE_PATH))
            {
                Picasso.With(this.ParentActivity)
                       .Load(Resource.Drawable.camera_snapshot)
                       .Into(imgProfile);
            }
            else
            {
                Picasso.With(ParentActivity)
                       .Load(ParentActivity.RegisterModel.FILE_PATH)
                       .Error(Resource.Drawable.camera_snapshot)
                       .Into(imgProfile);
            }
        }

        void BtNext_Click(object sender, EventArgs e)
        {
            if(StringCheckUtil.isEmpty(txtName))
            {
                return;
            }

            if (ProfileImageFile != null)
            {
                ParentActivity.RegisterModel.FILE_PATH = ProfileImageFile.Path;
                ParentActivity.RegisterModel.FILE_NAME = Path.GetFileName(ProfileImageFile.Path);
                ParentActivity.RegisterModel.FILE_STREAM = ProfileImageFile.GetStream();
            }

            ParentActivity.RegisterModel.USERNAME = txtName.Text;
            ParentActivity.DoRegister();
        }
    }
}
