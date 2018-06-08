using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Droid.Helpers;
using WhatMessenger.Model.BaseModel;
using WhatMessenger.Model.Constants;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.Account.Adapters
{
    public class BlockedContactListAdapter: BaseRecycleViewAdapter
    {
        ContactListViewModel ViewModel;
        Activity ParentActivity;
        List<ContactDTO> BlockedList;
        public BlockedContactListAdapter(Activity ParentActivity, ContactListViewModel ViewModel)
        {
            this.ParentActivity = ParentActivity;
            this.ViewModel = ViewModel;
            BlockedList = ViewModel.Items.Where<ContactDTO>(u => u.IS_I_BLOCKED).ToList();
            ViewModel.Items.CollectionChanged += (sender, args) =>
            {
                BlockedList = ViewModel.Items.Where<ContactDTO>(u => u.IS_I_BLOCKED).ToList();
                this.ParentActivity.RunOnUiThread(NotifyDataSetChanged);
            };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(Android.Views.ViewGroup parent, int viewType)
        {
            var itemView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_contact_history, parent, false);
            var holder = new BlockedListItemViewHolder(itemView, OnClick, OnLongClick);
            return holder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            (holder as BlockedListItemViewHolder).SetData(BlockedList[position]);
        }

        public override int ItemCount => BlockedList.Count;

    }

    public class BlockedListItemViewHolder : BaseViewHolder
    {
        View rootView;
        ImageViewAsync imgView;
        TextView txtName, txtEmail;

        public BlockedListItemViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener) : base(itemView, ClickListener, LongClickListener)
        {
            rootView = itemView;
            imgView = itemView.FindViewById<ImageViewAsync>(Resource.Id.imgProfile);
            txtName = itemView.FindViewById<TextView>(Resource.Id.txtName);
            txtEmail = itemView.FindViewById<TextView>(Resource.Id.txtEmail);
        }


        public void SetData(ContactDTO model)
        {
            if (string.IsNullOrEmpty(model.PIC))
            {
                ImageService.Instance.LoadCompiledResource("female_placeholder")
                            .Retry(3, 200)
                            .IntoAsync(imgView);

            }
            else
            {
                ImageService.Instance.LoadUrl(ServerURL.BaseURL + model.PIC)
                            .DownSample(width: 180)
                            .Retry(3, 200)
                            .Into(imgView);
            }
            txtName.Text = model.NAME.Trim();
            txtEmail.Text = model.EMAIL.Trim();

           
        }
    }

}
