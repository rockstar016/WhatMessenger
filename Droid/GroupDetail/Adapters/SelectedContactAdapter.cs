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
    public class SelectedContactAdapter: BaseRecycleViewAdapter
    {
        GroupListViewModel ViewModel;
        Activity Activity;
        public SelectedContactAdapter(Activity Activity, GroupListViewModel ViewModel)
        {
            this.ViewModel = ViewModel;
            this.Activity = Activity;
            this.ViewModel.ChoosedContactList.CollectionChanged += (sender, e) => {
                Activity.RunOnUiThread(() => { NotifyDataSetChanged(); });
            };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(Android.Views.ViewGroup parent, int viewType)
        {

            var itemView = LayoutInflater.From(this.Activity).Inflate(Resource.Layout.item_group_choosed_contact, parent, false);
            var holder = new SelectedContactViewHolder(itemView, OnClick, OnLongClick);
            return holder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            (holder as SelectedContactViewHolder).SetData(ViewModel.ChoosedContactList[position]);
        }

        public override int ItemCount => ViewModel.ChoosedContactList.Count;
    }

    public class SelectedContactViewHolder : RecyclerView.ViewHolder
    {
        ImageViewAsync imgProfile;
        TextView txtUserName;
        ImageButton imgCheck;
        public SelectedContactViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener) : base(itemView)
        {
            itemView.Click += (sender, e) => ClickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => LongClickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            imgProfile = itemView.FindViewById<ImageViewAsync>(Resource.Id.imgProfile);
            txtUserName = itemView.FindViewById<TextView>(Resource.Id.txtUserName);
            imgCheck = itemView.FindViewById<ImageButton>(Resource.Id.imgCheck);
        }

        public void SetData(ChoosableContact DataModel)
        {
            txtUserName.Text = DataModel.ContactDTO.NAME.Trim();

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
