using System;
using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using WhatMessenger.Model;
using WhatMessenger.Model.Constants;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.GroupDetail.Adapters
{
    public class ParticipantsAdapter : BaseRecycleViewAdapter
    {
        GroupListViewModel ViewModel;
        Activity Activity;
        public ParticipantsAdapter(Activity Activity, GroupListViewModel ViewModel)
        {
            this.ViewModel = ViewModel;
            this.Activity = Activity;
            this.ViewModel.ChoosedContactList.CollectionChanged += (sender, e) =>
            {
                Activity.RunOnUiThread(() => { NotifyDataSetChanged(); });
            };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(Android.Views.ViewGroup parent, int viewType)
        {

            var itemView = LayoutInflater.From(this.Activity).Inflate(Resource.Layout.item_group_choosed_contact, parent, false);
            var holder = new GroupCandidateViewHolder(itemView, OnClick, OnLongClick);
            return holder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            (holder as GroupCandidateViewHolder).SetData(ViewModel.ChoosedContactList[position]);
        }

        public override int ItemCount => ViewModel.ChoosedContactList.Count;

    }

    public class GroupCandidateViewHolder : RecyclerView.ViewHolder
    {
        ImageViewAsync imgProfile;
        TextView txtUserName;
        ImageButton imgCheck;
        public GroupCandidateViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener) : base(itemView)
        {
            imgProfile = itemView.FindViewById<ImageViewAsync>(Resource.Id.imgProfile);
            TxtUserName = itemView.FindViewById<TextView>(Resource.Id.txtUserName);
            imgCheck = itemView.FindViewById<ImageButton>(Resource.Id.imgCheck);
        }

        public TextView TxtUserName { get => txtUserName; set => txtUserName = value; }

        public void SetData(ChoosableContact DataModel)
        {
            TxtUserName.Text = DataModel.ContactDTO.NAME.Trim();
            imgCheck.Visibility = ViewStates.Invisible;
            if (string.IsNullOrEmpty(DataModel.ContactDTO.PIC))
            {
                ImageService.Instance.LoadCompiledResource("female_placeholder")
                            .Retry(3, 200)
                            .Transform(new CircleTransformation())
                            .IntoAsync(imgProfile);

            }
            else
            {
                ImageService.Instance.LoadUrl(ServerURL.BaseURL + DataModel.ContactDTO.PIC)
                            .Retry(3, 200)
                            .Transform(new CircleTransformation())
                            .ErrorPlaceholder("female_placeholder")
                            .Into(imgProfile);
            }
        }
    }
}
