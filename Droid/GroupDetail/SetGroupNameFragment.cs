
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Refractored.Controls;
using Rock.Utils;
using Square.Picasso;
using WhatMessenger.Droid.Engine;
using WhatMessenger.Droid.GroupDetail.Adapters;
using WhatMessenger.Model.RequestModels;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.GroupDetail
{
    public class SetGroupNameFragment : GroupBaseFragment
    {
        public static SetGroupNameFragment GetInstance() => new SetGroupNameFragment { Arguments = new Bundle() };
        FloatingActionButton fabNext;
        CircleImageView imgProfile;
        EditText txtGroupName;
        TextView txtParticipants;
        RecyclerView recyclerContent;
        GroupListViewModel ViewModel;

        MediaFile ChoosePhotoFile;
        ParticipantsAdapter recyclerAdapter;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewModel = EngineService.EngineInstance.GroupListViewModel;
            var rootView = inflater.Inflate(Resource.Layout.fragment_group_setname, container, false);

            imgProfile = rootView.FindViewById<CircleImageView>(Resource.Id.profile_image);
            txtGroupName = rootView.FindViewById<EditText>(Resource.Id.txtGroupName);
            recyclerContent = rootView.FindViewById<RecyclerView>(Resource.Id.recyclerContent);
            recyclerAdapter = new ParticipantsAdapter(ParentActivity, ViewModel);
            var gridLayoutManager = new GridLayoutManager(ParentActivity, 3);
            recyclerContent.SetLayoutManager(gridLayoutManager);
            recyclerContent.SetAdapter(recyclerAdapter);

            txtParticipants = rootView.FindViewById<TextView>(Resource.Id.txtParticipants);
            fabNext = rootView.FindViewById<FloatingActionButton>(Resource.Id.fabNext);
            fabNext.Click += FabNext_Click;
            imgProfile.Click += ImgProfile_Click;
            return rootView;
        }

        #region Open Camera/Gallery
        void ImgProfile_Click(object sender, EventArgs e)
        {
            var photoOptions = new string[] { @"From Gallery", @"Take Photo" };
            new Android.Support.V7.App.AlertDialog.Builder(this.Context)
                           .SetTitle("Choose Options")
                           .SetItems(photoOptions, (sender1, e1) =>
                           {
                               if (e1.Which == 0)
                               {
                                    ParentActivity.RunOnUiThread(() => {
                                        OpenGallery();
                                    });
                               }
                               else
                               {
                                   ParentActivity.RunOnUiThread(() =>
                                   {
                                       OpenCamera();
                                   });
                               }
                           })
                           .Create()
                           .Show();
        }

        async void OpenGallery()
        {
            var PermStorage = await CheckNecessaryPermissions(Plugin.Permissions.Abstractions.Permission.Storage);
            if (PermStorage)
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsPickPhotoSupported)
                {
                    DialogUtils.ShowOKDialog(this.Activity, @"Warning", @"No camera available.");
                    return;
                }

                ViewModel.GroupAvatar = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small
                });

                if (ViewModel.GroupAvatar != null)
                {
                    Picasso.With(this.ParentActivity)
                           .Load(new Java.IO.File(ViewModel.GroupAvatar.Path))
                           .Error(Resource.Drawable.camera_snapshot)
                           .Into(imgProfile);
                }
            }
            else
            {
                ViewModel.GroupAvatar = null;
            }
        }

        async void OpenCamera()
        {
            try
            {
                var PermStorage = await CheckNecessaryPermissions(Plugin.Permissions.Abstractions.Permission.Storage);
                var CameraStorage = await CheckNecessaryPermissions(Plugin.Permissions.Abstractions.Permission.Camera);
                if (PermStorage && CameraStorage)
                {
                    if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                    {
                        DialogUtils.ShowOKDialog(this.Activity, "Warning", "No camera available.");
                        return;
                    }

                    ViewModel.GroupAvatar = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                    {
                        Directory = "Profile",
                        Name = DateTime.Now.ToShortDateString() + ".jpg",
                        PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small
                    });

                    if (ViewModel.GroupAvatar != null)
                    {
                        Picasso.With(this.ParentActivity)
                               .Load(new Java.IO.File(ViewModel.GroupAvatar.Path))
                               .Error(Resource.Drawable.camera_snapshot)
                               .Into(imgProfile);
                    }
                }
                else
                {
                    ViewModel.GroupAvatar = null;
                }
            }
            catch(Exception e)
            {
                
            }
        }
        #endregion
        public override void OnStart()
        {
            base.OnStart();
            ParentActivity.SetPrimaryTextContent("New Group");
            ParentActivity.SetSecondaryTextContent("Add subject");
            txtParticipants.Text = $"Participants : {ViewModel.ChoosedContactList.Count}";
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(string.Equals(e.PropertyName, nameof(GroupListViewModel.IsBusy)))
            {
                if(ViewModel.IsBusy)
                {
                    ShowLoadingDialog("Adding Group");
                }
                else
                {
                    HideLoadingDialog();
                    this.ParentActivity.Finish();
                }
            }
        }

        public override void OnStop()
        {
            base.OnStop();
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }


        void FabNext_Click(object sender, EventArgs e)
        {
            if (StringCheckUtil.isEmpty(txtGroupName))
            {
                return;
            }

            var model = new GroupAddRequest()
            {
                GROUP_NAME = txtGroupName.Text.Trim(),
                MY_USER_ID = ParentActivity.MyApplication.Me.USERID,
                TOKEN = ParentActivity.MyApplication.Me.TOKEN, 
                OWNER_NAME = ParentActivity.MyApplication.Me.NAME
            };

            ViewModel.AddGroupItemCommand.Execute(model);
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
