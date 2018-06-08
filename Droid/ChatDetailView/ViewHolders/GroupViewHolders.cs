using System;
using Android.Support.V7.Widget;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Model.BaseModel;
using WhatMessenger.Model.Constants;
using WhatMessenger.ViewModel.Utils;

namespace WhatMessenger.Droid.ChatDetailView
{
    public class GroupViewHolders
    {
        public GroupViewHolders()
        {
        }
    }

    public class GroupReceiveImageViewHolder : BaseViewHolder, IChatViewHolder<GroupHistoryItemDTO>
    {
        ImageViewAsync imgContent;
        TextView txtDate, senderName;

        public GroupReceiveImageViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener) : base(itemView, ClickListener, LongClickListener)
        {
            imgContent = itemView.FindViewById<ImageViewAsync>(Resource.Id.txtContent);
            txtDate = itemView.FindViewById<TextView>(Resource.Id.txtTimeStamp);
            senderName = itemView.FindViewById<TextView>(Resource.Id.senderName);
        }

        public void SetData(GroupHistoryItemDTO model)
        {
            senderName.Text = model.SENDER_NAME.Trim();
            if (string.IsNullOrEmpty(model.CONTENT))
            {
                ImageService.Instance.LoadCompiledResource("placeholder_img_thumb")
                            .ErrorPlaceholder("placeholder_img_thumb", FFImageLoading.Work.ImageSource.CompiledResource)
                            .LoadingPlaceholder("placeholder_img_thumb", FFImageLoading.Work.ImageSource.CompiledResource)
                        .Retry(3, 200)
                        .Into(imgContent);
                var dateTime = DateConverter.GetDateTimeFromUnixTimeStamp(model.CREATED_AT);
                txtDate.Text = dateTime.ToString("yyyy-MM-dd hh:mm");
            }
            else
            {
                ImageService.Instance.LoadUrl(ServerURL.BaseURL + model.CONTENT)
                            .DownSample(width: 180)
                            .ErrorPlaceholder("placeholder_img_thumb", FFImageLoading.Work.ImageSource.CompiledResource)
                            .LoadingPlaceholder("placeholder_img_thumb", FFImageLoading.Work.ImageSource.CompiledResource)
                        .Retry(3, 200)
                        .Into(imgContent);
                var dateTime = DateConverter.GetDateTimeFromUnixTimeStamp(model.CREATED_AT);
                txtDate.Text = dateTime.ToString("yyyy-MM-dd hh:mm");
            }
        }
    }

    public class GroupReceiveTextViewHolder : BaseViewHolder, IChatViewHolder<GroupHistoryItemDTO>
    {
        TextView txtContent, txtTimeStamp, senderName;
        public GroupReceiveTextViewHolder(Android.Views.View itemView,Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener) : base(itemView, ClickListener, LongClickListener)
        {
            txtContent = itemView.FindViewById<TextView>(Resource.Id.txtContent);
            txtTimeStamp = itemView.FindViewById<TextView>(Resource.Id.txtTimeStamp);
            senderName = itemView.FindViewById<TextView>(Resource.Id.senderName);
        }

        public void SetData(GroupHistoryItemDTO model)
        {
            senderName.Text = model.SENDER_NAME.Trim();
            txtContent.Text = model.CONTENT.Trim();
            var dateTime = DateConverter.GetDateTimeFromUnixTimeStamp(model.CREATED_AT);
            txtTimeStamp.Text = dateTime.ToString("yyyy-MM-dd hh:mm");
        }
    }

    public class GroupReceivePdfViewHolder : BaseViewHolder, IChatViewHolder<GroupHistoryItemDTO>
    {
        TextView txtTimeStamp, senderName;
        ImageView imgFavoriteIndicator;
        public GroupReceivePdfViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener) : base(itemView, ClickListener, LongClickListener)
        {
            imgFavoriteIndicator = itemView.FindViewById<ImageView>(Resource.Id.imgFavoriteIndicator);
            txtTimeStamp = itemView.FindViewById<TextView>(Resource.Id.txtTimeStamp);
            senderName = itemView.FindViewById<TextView>(Resource.Id.senderName);
        }

        public void SetData(GroupHistoryItemDTO model)
        {
            var dateTime = DateConverter.GetDateTimeFromUnixTimeStamp(model.CREATED_AT);
            txtTimeStamp.Text = dateTime.ToString("yyyy-MM-dd hh:mm");
            senderName.Text = model.SENDER_NAME.Trim();
        }
    }

    public class GroupReceiveVideoViewHolder : BaseViewHolder, IChatViewHolder<GroupHistoryItemDTO>
    {
        TextView txtTimeStamp, senderName;
        ImageView imgFavoriteIndicator;
        public GroupReceiveVideoViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener) : base(itemView, ClickListener, LongClickListener)
        {
            imgFavoriteIndicator = itemView.FindViewById<ImageView>(Resource.Id.imgFavoriteIndicator);
            txtTimeStamp = itemView.FindViewById<TextView>(Resource.Id.txtTimeStamp);
            senderName = itemView.FindViewById<TextView>(Resource.Id.senderName);
        }

        public void SetData(GroupHistoryItemDTO model)
        {
            var dateTime = DateConverter.GetDateTimeFromUnixTimeStamp(model.CREATED_AT);
            txtTimeStamp.Text = dateTime.ToString("yyyy-MM-dd hh:mm");
            senderName.Text = model.SENDER_NAME.Trim();
        }
    }

    public class GroupReceiveAudioViewHolder : BaseViewHolder, IChatViewHolder<GroupHistoryItemDTO>
    {
        TextView txtTimeStamp, senderName;
        ImageView imgFavoriteIndicator;

        public GroupReceiveAudioViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener) : base(itemView, ClickListener, LongClickListener)
        {
            imgFavoriteIndicator = itemView.FindViewById<ImageView>(Resource.Id.imgFavoriteIndicator);
            txtTimeStamp = itemView.FindViewById<TextView>(Resource.Id.txtTimeStamp);
            senderName = itemView.FindViewById<TextView>(Resource.Id.senderName);
        }

        public void SetData(GroupHistoryItemDTO model)
        {
            var dateTime = DateConverter.GetDateTimeFromUnixTimeStamp(model.CREATED_AT);
            txtTimeStamp.Text = dateTime.ToString("yyyy-MM-dd hh:mm");
            senderName.Text = model.SENDER_NAME.Trim();
        }
    }


    public class GroupSendTextViewHolder : BaseViewHolder, IChatViewHolder<GroupHistoryItemDTO>
    {
        TextView txtContent, txtTimeStamp;
        public GroupSendTextViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener) : base(itemView, ClickListener, LongClickListener)
        {
            txtContent = itemView.FindViewById<TextView>(Resource.Id.txtContent);
            txtTimeStamp = itemView.FindViewById<TextView>(Resource.Id.txtTimeStamp);
        }

        public void SetData(GroupHistoryItemDTO model)
        {
            txtContent.Text = model.CONTENT.Trim();
            var dateTime = DateConverter.GetDateTimeFromUnixTimeStamp(model.CREATED_AT);
            txtTimeStamp.Text = dateTime.ToString("yyyy-MM-dd hh:mm");
        }
    }

    public class GroupSendPdfViewHolder : BaseViewHolder, IChatViewHolder<GroupHistoryItemDTO>
    {
        TextView txtTimeStamp;
        ImageView imgSendStatus, txtContent;
        FrameLayout containerCancel;
        ImageButton btSendCancel;
        public GroupSendPdfViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener, Action<RecyclerClickEventArgs> CancelClickListener) : base(itemView, ClickListener, LongClickListener)
        {
            imgSendStatus = itemView.FindViewById<ImageView>(Resource.Id.srcStatus);
            txtTimeStamp = itemView.FindViewById<TextView>(Resource.Id.txtTimeStamp);
            txtContent = itemView.FindViewById<ImageView>(Resource.Id.txtContent);
            containerCancel = itemView.FindViewById<FrameLayout>(Resource.Id.containerCancel);
            btSendCancel = itemView.FindViewById<ImageButton>(Resource.Id.btSendCancel);
            btSendCancel.Click += (sender, e) => CancelClickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
        }

        public void SetData(GroupHistoryItemDTO model)
        {
            var dateTime = DateConverter.GetDateTimeFromUnixTimeStamp(model.CREATED_AT);
            txtTimeStamp.Text = dateTime.ToString("yyyy-MM-dd hh:mm");
            containerCancel.Visibility = string.IsNullOrEmpty(model.CONTENT.Trim()) ? Android.Views.ViewStates.Invisible : Android.Views.ViewStates.Visible;
            txtContent.Alpha = !string.IsNullOrEmpty(model.CONTENT.Trim()) ? 1.0f : 0.4f;
        }
    }

    public class GroupSendVideoViewHolder : BaseViewHolder, IChatViewHolder<GroupHistoryItemDTO>
    {
        TextView txtTimeStamp;
        ImageView imgSendStatus, txtContent;
        FrameLayout containerCancel;
        ImageButton btSendCancel;
        public GroupSendVideoViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener, Action<RecyclerClickEventArgs> CancelClickListener) : base(itemView, ClickListener, LongClickListener)
        {
            imgSendStatus = itemView.FindViewById<ImageView>(Resource.Id.srcStatus);
            txtTimeStamp = itemView.FindViewById<TextView>(Resource.Id.txtTimeStamp);
            containerCancel = itemView.FindViewById<FrameLayout>(Resource.Id.containerCancel);
            txtContent = itemView.FindViewById<ImageView>(Resource.Id.txtContent);
            btSendCancel = itemView.FindViewById<ImageButton>(Resource.Id.btSendCancel);
            btSendCancel.Click += (sender, e) => CancelClickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
        }

        public void SetData(GroupHistoryItemDTO model)
        {
            var dateTime = DateConverter.GetDateTimeFromUnixTimeStamp(model.CREATED_AT);
            txtTimeStamp.Text = dateTime.ToString("yyyy-MM-dd hh:mm");
            containerCancel.Visibility = !string.IsNullOrEmpty(model.CONTENT.Trim()) ? Android.Views.ViewStates.Invisible : Android.Views.ViewStates.Visible;
            txtContent.Alpha = !string.IsNullOrEmpty(model.CONTENT.Trim()) ? 1.0f : 0.4f;
        }
    }

    public class GroupSendAudioViewHolder : BaseViewHolder, IChatViewHolder<GroupHistoryItemDTO>
    {
        TextView txtTimeStamp;
        ImageView imgSendStatus, txtContent;
        FrameLayout containerCancel;
        ImageButton btSendCancel;
        public GroupSendAudioViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener, Action<RecyclerClickEventArgs> CancelClickListener) : base(itemView, ClickListener, LongClickListener)
        {
            imgSendStatus = itemView.FindViewById<ImageView>(Resource.Id.srcStatus);
            txtTimeStamp = itemView.FindViewById<TextView>(Resource.Id.txtTimeStamp);
            containerCancel = itemView.FindViewById<FrameLayout>(Resource.Id.containerCancel);
            txtContent = itemView.FindViewById<ImageView>(Resource.Id.txtContent);
            btSendCancel = itemView.FindViewById<ImageButton>(Resource.Id.btSendCancel);
            btSendCancel.Click += (sender, e) => CancelClickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
        }

        public void SetData(GroupHistoryItemDTO model)
        {
            var dateTime = DateConverter.GetDateTimeFromUnixTimeStamp(model.CREATED_AT);
            txtTimeStamp.Text = dateTime.ToString("yyyy-MM-dd hh:mm");
            containerCancel.Visibility = !string.IsNullOrEmpty(model.CONTENT.Trim()) ? Android.Views.ViewStates.Invisible : Android.Views.ViewStates.Visible;
            txtContent.Alpha = !string.IsNullOrEmpty(model.CONTENT.Trim()) ? 1.0f : 0.4f;
        }
    }

    public class GroupSendImageViewHolder : BaseViewHolder, IChatViewHolder<GroupHistoryItemDTO>
    {
        ImageViewAsync imgContent;
        TextView txtDate;
        ImageView imgReadStatus;
        FrameLayout containerCancel;
        ImageButton btSendCancel;

        public GroupSendImageViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener, Action<RecyclerClickEventArgs> CancelClickListener) : base(itemView, ClickListener, LongClickListener)
        {
            imgContent = itemView.FindViewById<ImageViewAsync>(Resource.Id.txtContent);
            txtDate = itemView.FindViewById<TextView>(Resource.Id.txtTimeStamp);
            containerCancel = itemView.FindViewById<FrameLayout>(Resource.Id.containerCancel);
            btSendCancel = itemView.FindViewById<ImageButton>(Resource.Id.btSendCancel);
            btSendCancel.Click += (sender, e) => CancelClickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
        }

        public void SetData(GroupHistoryItemDTO model)
        {
            if (string.IsNullOrEmpty(model.CONTENT))
            {
                containerCancel.Visibility = Android.Views.ViewStates.Visible;
                ImageService.Instance.LoadCompiledResource("placeholder_img_thumb")
                            .ErrorPlaceholder("placeholder_img_thumb", FFImageLoading.Work.ImageSource.CompiledResource)
                            .LoadingPlaceholder("placeholder_img_thumb", FFImageLoading.Work.ImageSource.CompiledResource)
                            .Retry(3, 200)
                            .Into(imgContent);
                imgContent.Alpha = 0.4f;
                var dateTime = DateConverter.GetDateTimeFromUnixTimeStamp(model.CREATED_AT);
                txtDate.Text = dateTime.ToString("yyyy-MM-dd hh:mm");
            }
            else
            {
                containerCancel.Visibility = Android.Views.ViewStates.Invisible;
                ImageService.Instance.LoadUrl(ServerURL.BaseURL + model.CONTENT)
                            .DownSample(width: 180)
                            .ErrorPlaceholder("placeholder_img_thumb", FFImageLoading.Work.ImageSource.CompiledResource)
                            .LoadingPlaceholder("placeholder_img_thumb", FFImageLoading.Work.ImageSource.CompiledResource)
                        .Retry(3, 200)
                        .Into(imgContent);
                imgContent.Alpha = 1.0f;
                var dateTime = DateConverter.GetDateTimeFromUnixTimeStamp(model.CREATED_AT);
                txtDate.Text = dateTime.ToString("yyyy-MM-dd hh:mm");
            }
        }
    }
}
