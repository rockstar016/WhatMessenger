using System;
using System.Collections.Specialized;
using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Model;
using WhatMessenger.Model.BaseModel;
using WhatMessenger.Model.Constants;
using WhatMessenger.ViewModel;
using WhatMessenger.ViewModel.Utils;

namespace WhatMessenger.Droid.Fragments
{
    public class ChatListAdapter : BaseRecycleViewAdapter
    {
        ChatListViewModel ViewModel;
        Activity ParentActivity;
        public ChatListAdapter(Activity ParentActivity, ChatListViewModel ViewModel)
        {
            this.ParentActivity = ParentActivity;
            this.ViewModel = ViewModel;

            ViewModel.PrivateChatEntryCollection.CollectionChanged += (sender, args) =>
            {
                this.ParentActivity.RunOnUiThread(NotifyDataSetChanged);
            };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(Android.Views.ViewGroup parent, int viewType)
        {
            var itemView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_chat_history, parent, false);
            var holder = new ChatHistoryItemViewHolder(itemView, OnClick, OnLongClick);
            return holder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var ItemModel = ViewModel.PrivateChatEntryCollection[position];
            (holder as ChatHistoryItemViewHolder).InitDataView(ItemModel);
            ParentActivity.RunOnUiThread(()=>{
                //if (ItemModel.IsSelected)
                //{
                //    (holder as ChatHistoryItemViewHolder).chkSelected.Alpha = 1f;
                //}
                //else
                //{
                    (holder as ChatHistoryItemViewHolder).chkSelected.Alpha = 0f;
                //}    
            });

        }


        public override int ItemCount => ViewModel.PrivateChatEntryCollection.Count;

    }

    public class ChatHistoryItemViewHolder : BaseViewHolder
    {
        public ImageView chkSelected { get; set; }
        public ImageViewAsync imgProfile { get; set; }
        public ImageView chkStatus { get; set; }

        TextView txtName, txtDescription, txtTime, txtNotification, txtDate;
        public ChatHistoryItemViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener) : base(itemView, ClickListener, LongClickListener)
        {
            chkSelected = itemView.FindViewById<ImageView>(Resource.Id.chkSelected);
            chkStatus = itemView.FindViewById<ImageView>(Resource.Id.chkStatus);
            txtName = itemView.FindViewById<TextView>(Resource.Id.txtName);
            txtDescription = itemView.FindViewById<TextView>(Resource.Id.txtDescription);
            txtTime = itemView.FindViewById<TextView>(Resource.Id.txtTime);
            txtNotification = itemView.FindViewById<TextView>(Resource.Id.txtNotification);
            imgProfile = itemView.FindViewById<ImageViewAsync>(Resource.Id.imgProfile);
            txtDate = itemView.FindViewById<TextView>(Resource.Id.txtDate);
        }

        public void InitDataView(ChatEntryDTO ItemModel)
        {
            if(Convert.ToInt16(ItemModel.UnreadMessageCount) == 0)
            {
                txtNotification.Visibility = ViewStates.Invisible;
            }
            else
            {
                txtNotification.Visibility = ViewStates.Visible;
            }

            txtNotification.Text = $"{ItemModel.UnreadMessageCount}";

            txtName.Text = ItemModel.OtherUserName;
            var dateValue = DateConverter.GetDateTimeFromUnixTimeStamp(ItemModel.LastUpdateDate);

            txtTime.Text = dateValue.ToString("hh:mm");
            txtDate.Text = dateValue.ToString("yyyy-MM-dd");
            txtDescription.Text = ItemModel.LastMessage;

            if (string.IsNullOrEmpty(ItemModel.OtherUserPic))
            {
                ImageService.Instance.LoadCompiledResource("female_placeholder")
                            .Retry(3, 200)
                            .IntoAsync(imgProfile);

            }
            else
            {
                ImageService.Instance.LoadUrl(ServerURL.BaseURL + ItemModel.OtherUserPic)
                            .DownSample(width: 150)
                            .Retry(3, 200)
                            .ErrorPlaceholder("female_placeholder")
                            .Into(imgProfile);
            }
            chkStatus.SetImageResource(ItemModel.OtherUserOnlineStatus ? Resource.Drawable.ic_circle_white_green : Resource.Drawable.ic_circle_green);
            if (ItemModel.IsBlocked || !ItemModel.OtherUserActivate) chkStatus.SetImageResource(Resource.Drawable.ic_circle_green);
        }
    }
}
