using System;
using System.Collections.Generic;
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
    public class ChooseContactAdapter : BaseRecycleViewAdapter
    {
        GroupListViewModel ViewModel;
        Activity Activity;
        public ChooseContactAdapter(Activity Activity, GroupListViewModel GroupListViewModel)
        {
            this.Activity = Activity;
            this.ViewModel = GroupListViewModel;
            GroupListViewModel.ChoosableContactList.CollectionChanged += (sender, e) => {
                    this.Activity.RunOnUiThread(NotifyDataSetChanged);
            };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(Android.Views.ViewGroup parent, int viewType)
        {
            var itemView = LayoutInflater.From(this.Activity).Inflate(Resource.Layout.item_group_additem, parent, false);
            var holder = new ChooseContactViewHolder(itemView, OnClick, OnLongClick);
            return holder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            (holder as ChooseContactViewHolder).SetData(ViewModel.ChoosableContactList[position]);
        }

        public override int ItemCount => ViewModel.ChoosableContactList.Count;

    }

    public class ChooseContactViewHolder : RecyclerView.ViewHolder
    {
        ImageView imgChoose;
        TextView txtUserName, txtUserStatus;
        ImageViewAsync imgProfile;
        public ChooseContactViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener) : base(itemView)
        {
            itemView.Click += (sender, e) => ClickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => LongClickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            imgChoose = itemView.FindViewById<ImageView>(Resource.Id.imgCheck);
            txtUserName = itemView.FindViewById<TextView>(Resource.Id.txtUserName);
            txtUserStatus = itemView.FindViewById<TextView>(Resource.Id.txtStatus);
            imgProfile = itemView.FindViewById<ImageViewAsync>(Resource.Id.imgProfile);
        }

        public void SetData(ChoosableContact DataModel)
        {
            imgChoose.Visibility = DataModel.Choose ? ViewStates.Visible : ViewStates.Invisible;
            txtUserName.Text = DataModel.ContactDTO.NAME;
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
