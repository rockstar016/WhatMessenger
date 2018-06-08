using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using API.Models.RequestModels;
using Plugin.Media;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Rock.Utils;
using Square.Picasso;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Droid.Engine;
using WhatMessenger.Droid.GroupDetail.Adapters;
using WhatMessenger.Model.BaseModel;
using WhatMessenger.Model.Constants;
using WhatMessenger.ViewModel;
using static Android.Manifest;

namespace WhatMessenger.Droid.GroupDetail
{
    [Activity(Label = "")]
    public class EditGroupActivity: BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_edit_group;
        Android.Support.V7.Widget.Toolbar toolbar;
        ImageView imgProfile;
        FloatingActionButton fab;
        TextView txtTotalIndicator, txtTotalCount, txtAdminIndicator, txtAdminName, txtGroupName, txtGroupNameIndicator;
        RecyclerView recyclerMember;
        ImageButton btEditGroupName;
        Button btLeaveGroup, btAddMember;

        GroupListViewModel GroupViewModel;
        GroupEditAdapter adapter;
		protected override void OnCreate(Bundle savedInstanceState)
		{
            GroupViewModel = EngineService.EngineInstance.GroupListViewModel;
            base.OnCreate(savedInstanceState);
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            imgProfile = FindViewById<ImageView>(Resource.Id.imgProfile);
            fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            txtTotalIndicator = FindViewById<TextView>(Resource.Id.txtTotalIndicator);
            txtTotalCount = FindViewById<TextView>(Resource.Id.txtTotalCount);
            txtAdminIndicator = FindViewById<TextView>(Resource.Id.txtAdminIndicator);
            txtAdminName = FindViewById<TextView>(Resource.Id.txtAdminName);
            txtGroupName = FindViewById<TextView>(Resource.Id.txtGroupName);
            txtGroupNameIndicator = FindViewById<TextView>(Resource.Id.txtGroupNameIndicator);
            recyclerMember = FindViewById<RecyclerView>(Resource.Id.recyclerContacts);
            var linearLayoutManager = new CustomLinearLayoutManager(this);
            linearLayoutManager.setScrollEnabled(false);
            recyclerMember.SetLayoutManager(linearLayoutManager);
            adapter = new GroupEditAdapter(this, new List<UserDTO>());
            recyclerMember.SetAdapter(adapter);
            btEditGroupName = FindViewById<ImageButton>(Resource.Id.btEditGroupName);
            btLeaveGroup = FindViewById<Button>(Resource.Id.btLeaveGroup);
            btAddMember = FindViewById<Button>(Resource.Id.btAddMember);
            btEditGroupName.Click += BtEditGroupName_Click;
            fab.Click += ChangePhoto_Click;
            btAddMember.Click += BtAddMember_Click;
            btLeaveGroup.Click += BtLeaveGroup_Click;
	    }

        void BtLeaveGroup_Click(object sender, EventArgs e)
        {
            if(GroupViewModel.CurrentlyOpenDTO.GROUP_OWNER_ID == GetMyUserId())
            {
                //remove group
                GroupViewModel.CloseCurrentGroupCommand.Execute(MyApplication.Me.TOKEN);
            }
            else
            {
                //update group member list
                GroupViewModel.LeaveCurrentGroupMemberCommand.Execute(new GetProfileRequest() { TOKEN = MyApplication.Me.TOKEN, USERID = $"{GetMyUserId()}"});
            }
        }

        void BtAddMember_Click(object sender, EventArgs e)
        {
            var mIntent = new Intent(this, typeof(ActivityEditGroupMember));
            StartActivity(mIntent);
        }

#region Change Group Photo 
        private void ChangePhoto_Click(object sender, EventArgs e)
        {
            var photoOptions = new string[] { @"From Gallery", @"Take Photo" };
            new Android.Support.V7.App.AlertDialog.Builder(this)
                           .SetTitle("Choose Options")
                           .SetItems(photoOptions, (sender1, e1) =>
                           {
                               if (e1.Which == 0)
                               {
                                   RunOnUiThread(() => {
                                       OpenGallery();
                                   });
                               }
                               else
                               {
                                   RunOnUiThread(() =>
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
                    DialogUtils.ShowOKDialog(this, @"Warning", @"No camera available.");
                    return;
                }

                GroupViewModel.GroupAvatar = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small
                });

                if (GroupViewModel.GroupAvatar != null)
                {
                    Picasso.With(this)
                           .Load(new Java.IO.File(GroupViewModel.GroupAvatar.Path))
                           .Error(Resource.Drawable.camera_snapshot)
                           .Into(imgProfile);
                    GroupViewModel.ChangeCurrentGroupAvatarCommand.Execute(MyApplication.Me.TOKEN);
                }
            }
            else
            {
                GroupViewModel.GroupAvatar = null;
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
                        DialogUtils.ShowOKDialog(this, "Warning", "No camera available.");
                        return;
                    }

                    GroupViewModel.GroupAvatar = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                    {
                        Directory = "Profile",
                        Name = DateTime.Now.ToShortDateString() + ".jpg",
                        PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small
                    });

                    if (GroupViewModel.GroupAvatar != null)
                    {
                        Picasso.With(this)
                               .Load(new Java.IO.File(GroupViewModel.GroupAvatar.Path))
                               .Error(Resource.Drawable.camera_snapshot)
                               .Into(imgProfile);
                        GroupViewModel.ChangeCurrentGroupAvatarCommand.Execute(MyApplication.Me.TOKEN);
                    }
                }
                else
                {
                    GroupViewModel.GroupAvatar = null;
                }
            }
            catch (Exception e)
            {

            }
        }
        #endregion
        #region permission manage
        async Task<bool> CheckNecessaryPermissions(Plugin.Permissions.Abstractions.Permission permissionName)
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
        void BtEditGroupName_Click(object sender, EventArgs e)
        {
            var mIntent = new Intent(this, typeof(ActivityEditGroupName));
            StartActivity(mIntent);
        }

        void InitViews()
        {
            if (GroupViewModel.CurrentlyOpenDTO == null || string.IsNullOrEmpty(GroupViewModel.CurrentlyOpenDTO.GROUP_AVATAR))
            {
                Picasso.With(this).Load(Resource.Drawable.male_placeholder).Into(imgProfile);
            }
            else
            {
                Picasso.With(this).Load(ServerURL.BaseURL + GroupViewModel.CurrentlyOpenDTO.GROUP_AVATAR).Into(imgProfile);
            }
            txtTotalCount.Text = $"{GroupViewModel.CurrentlyOpenDTO.MEMBER_LIST.Count}";
            txtAdminName.Text = $"{GroupViewModel.CurrentlyOpenDTO.GROUP_OWNER_NAME}";
            txtGroupName.Text = $"{GroupViewModel.CurrentlyOpenDTO.GROUP_NAME}";
            adapter.Contactors = GroupViewModel.CurrentlyOpenDTO.MEMBER_LIST;
            adapter.NotifyDataSetChanged();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
            }
            return true;
        }

		protected override void OnStart()
		{
            base.OnStart();
            if(GroupViewModel == null)
            {
                EngineService.EngineInstance.GroupListViewModel = new GroupListViewModel();
                GroupViewModel = EngineService.EngineInstance.GroupListViewModel;
            }
            GroupViewModel.PropertyChanged += GroupViewModel_PropertyChanged;
            ShowHideNecessaryItem();
            InitViews();
		}

        void ShowHideNecessaryItem()
        {
            if(GetMyUserId() == GroupViewModel.CurrentlyOpenDTO.GROUP_OWNER_ID)
            {
                fab.Visibility = ViewStates.Visible;
                btAddMember.Visibility = ViewStates.Visible;
                btEditGroupName.Visibility = ViewStates.Visible;
                btLeaveGroup.Text = "Close Group";
            }
            else
            {
                fab.Visibility = ViewStates.Invisible;
                btAddMember.Visibility = ViewStates.Invisible;
                btEditGroupName.Visibility = ViewStates.Invisible;
                btLeaveGroup.Text = "Leave Group";
            }
        }

        void GroupViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(string.Equals(e.PropertyName, nameof(GroupListViewModel.CurrentlyOpenDTO)))
            {
                if (GroupViewModel.CurrentlyOpenDTO != null) InitViews();
                else Finish();
            }
            if(string.Equals(e.PropertyName, nameof(GroupListViewModel.IsBusy)))
            {
                if(GroupViewModel.IsBusy)
                {
                    ShowLoadingDialog("Updating");
                }
                else
                {
                    HideLoadingDialog();
                }
            }
        }

		protected override void OnStop()
		{
            base.OnStop();
            GroupViewModel.PropertyChanged -= GroupViewModel_PropertyChanged;
		}
	}
}
