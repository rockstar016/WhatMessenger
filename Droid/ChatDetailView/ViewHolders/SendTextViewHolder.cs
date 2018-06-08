using System;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Model.BaseModel;
using WhatMessenger.Model.Constants;
using WhatMessenger.ViewModel.Utils;

namespace WhatMessenger.Droid.ChatDetailView
{
    public class SendTextViewHolder: BaseViewHolder, IChatViewHolder<ChatHistoryItemDTO>
    {
        TextView txtContent, txtTimeStamp;
        public SendTextViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener) : base(itemView, ClickListener, LongClickListener)
        {
            txtContent = itemView.FindViewById<TextView>(Resource.Id.txtContent);
            txtTimeStamp = itemView.FindViewById<TextView>(Resource.Id.txtTimeStamp);
        }

        public void SetData(ChatHistoryItemDTO model)
        {
            txtContent.Text = model.CONTENT.Trim();
            var dateTime = DateConverter.GetDateTimeFromUnixTimeStamp(model.CREATED_AT);
            txtTimeStamp.Text = dateTime.ToString("yyyy-MM-dd hh:mm");
        }
    }

    public class SendPdfViewHolder : BaseViewHolder, IChatViewHolder<ChatHistoryItemDTO>
    {
        TextView txtTimeStamp;
        ImageView imgSendStatus, txtContent;
        FrameLayout containerCancel;
        ImageButton btSendCancel;
        public SendPdfViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener, Action<RecyclerClickEventArgs> CancelClickListener) : base(itemView, ClickListener, LongClickListener)
        {
            imgSendStatus = itemView.FindViewById<ImageView>(Resource.Id.srcStatus);
            txtTimeStamp = itemView.FindViewById<TextView>(Resource.Id.txtTimeStamp);
            txtContent = itemView.FindViewById<ImageView>(Resource.Id.txtContent);
            containerCancel = itemView.FindViewById<FrameLayout>(Resource.Id.containerCancel);
            btSendCancel = itemView.FindViewById<ImageButton>(Resource.Id.btSendCancel);
            btSendCancel.Click += (sender, e) => CancelClickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
        }

        public void SetData(ChatHistoryItemDTO model)
        {
            var dateTime = DateConverter.GetDateTimeFromUnixTimeStamp(model.CREATED_AT);
            txtTimeStamp.Text = dateTime.ToString("yyyy-MM-dd hh:mm");
            containerCancel.Visibility = string.IsNullOrEmpty(model.CONTENT.Trim()) ? Android.Views.ViewStates.Invisible : Android.Views.ViewStates.Visible;
            txtContent.Alpha = !string.IsNullOrEmpty(model.CONTENT.Trim()) ? 1.0f : 0.4f;
        }
    }

    public class SendVideoViewHolder : BaseViewHolder, IChatViewHolder<ChatHistoryItemDTO>
    {
        TextView txtTimeStamp;
        ImageView imgSendStatus, txtContent;
        FrameLayout containerCancel;
        ImageButton btSendCancel;
        public SendVideoViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener, Action<RecyclerClickEventArgs> CancelClickListener) : base(itemView, ClickListener, LongClickListener)
        {
            imgSendStatus = itemView.FindViewById<ImageView>(Resource.Id.srcStatus);
            txtTimeStamp = itemView.FindViewById<TextView>(Resource.Id.txtTimeStamp);
            containerCancel = itemView.FindViewById<FrameLayout>(Resource.Id.containerCancel);
            txtContent = itemView.FindViewById<ImageView>(Resource.Id.txtContent);
            btSendCancel = itemView.FindViewById<ImageButton>(Resource.Id.btSendCancel);
            btSendCancel.Click += (sender, e) => CancelClickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
        }

        public void SetData(ChatHistoryItemDTO model)
        {
            var dateTime = DateConverter.GetDateTimeFromUnixTimeStamp(model.CREATED_AT);
            txtTimeStamp.Text = dateTime.ToString("yyyy-MM-dd hh:mm");
            containerCancel.Visibility = !string.IsNullOrEmpty(model.CONTENT.Trim()) ? Android.Views.ViewStates.Invisible : Android.Views.ViewStates.Visible;
            txtContent.Alpha = !string.IsNullOrEmpty(model.CONTENT.Trim()) ? 1.0f : 0.4f;
        }
    }

    public class SendAudioViewHolder : BaseViewHolder, IChatViewHolder<ChatHistoryItemDTO>
    {
        TextView txtTimeStamp;
        ImageView imgSendStatus, txtContent;
        FrameLayout containerCancel;
        ImageButton btSendCancel;
        public SendAudioViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener, Action<RecyclerClickEventArgs> CancelClickListener) : base(itemView, ClickListener, LongClickListener)
        {
            imgSendStatus = itemView.FindViewById<ImageView>(Resource.Id.srcStatus);
            txtTimeStamp = itemView.FindViewById<TextView>(Resource.Id.txtTimeStamp);
            containerCancel = itemView.FindViewById<FrameLayout>(Resource.Id.containerCancel);
            txtContent = itemView.FindViewById<ImageView>(Resource.Id.txtContent);
            btSendCancel = itemView.FindViewById<ImageButton>(Resource.Id.btSendCancel);
            btSendCancel.Click += (sender, e) => CancelClickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
        }

        public void SetData(ChatHistoryItemDTO model)
        {
            var dateTime = DateConverter.GetDateTimeFromUnixTimeStamp(model.CREATED_AT);
            txtTimeStamp.Text = dateTime.ToString("yyyy-MM-dd hh:mm");
            containerCancel.Visibility = !string.IsNullOrEmpty(model.CONTENT.Trim()) ? Android.Views.ViewStates.Invisible : Android.Views.ViewStates.Visible;
            txtContent.Alpha = !string.IsNullOrEmpty(model.CONTENT.Trim()) ? 1.0f : 0.4f;
        }
    }

    public class SendImageViewHolder : BaseViewHolder, IChatViewHolder<ChatHistoryItemDTO>
    {
        ImageViewAsync imgContent;
        TextView txtDate;
        ImageView imgReadStatus;
        FrameLayout containerCancel;
        ImageButton btSendCancel;

        public SendImageViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener, Action<RecyclerClickEventArgs> CancelClickListener) : base(itemView, ClickListener, LongClickListener)
        {
            imgContent = itemView.FindViewById<ImageViewAsync>(Resource.Id.txtContent);
            txtDate = itemView.FindViewById<TextView>(Resource.Id.txtTimeStamp);
            containerCancel = itemView.FindViewById<FrameLayout>(Resource.Id.containerCancel);
            btSendCancel = itemView.FindViewById<ImageButton>(Resource.Id.btSendCancel);
            btSendCancel.Click += (sender, e) => CancelClickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
        }

        public void SetData(ChatHistoryItemDTO model)
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

    public class ReceiveTextViewHolder : BaseViewHolder, IChatViewHolder<ChatHistoryItemDTO>
    {
        TextView txtContent, txtTimeStamp;
        ImageView imgFavoriteIndicator;
        public ReceiveTextViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener) : base(itemView, ClickListener, LongClickListener)
        {
            txtContent = itemView.FindViewById<TextView>(Resource.Id.txtContent);
            txtTimeStamp = itemView.FindViewById<TextView>(Resource.Id.txtTimeStamp);
            imgFavoriteIndicator = itemView.FindViewById<ImageView>(Resource.Id.imgFavoriteIndicator);
        }

        public void SetData(ChatHistoryItemDTO model)
        {
            txtContent.Text = model.CONTENT.Trim();
            var dateTime = DateConverter.GetDateTimeFromUnixTimeStamp(model.CREATED_AT);
            txtTimeStamp.Text = dateTime.ToString("yyyy-MM-dd hh:mm");
            imgFavoriteIndicator.Visibility = model.IS_FAVORITE ? Android.Views.ViewStates.Visible : Android.Views.ViewStates.Invisible;
        }
    }

    public class ReceivePdfViewHolder : BaseViewHolder, IChatViewHolder<ChatHistoryItemDTO>
    {
        TextView txtTimeStamp;
        ImageView imgFavoriteIndicator;
        public ReceivePdfViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener) : base(itemView, ClickListener, LongClickListener)
        {
            imgFavoriteIndicator = itemView.FindViewById<ImageView>(Resource.Id.imgFavoriteIndicator);
            txtTimeStamp = itemView.FindViewById<TextView>(Resource.Id.txtTimeStamp);
            imgFavoriteIndicator = itemView.FindViewById<ImageView>(Resource.Id.imgFavoriteIndicator);

        }

        public void SetData(ChatHistoryItemDTO model)
        {
            var dateTime = DateConverter.GetDateTimeFromUnixTimeStamp(model.CREATED_AT);
            txtTimeStamp.Text = dateTime.ToString("yyyy-MM-dd hh:mm");
            imgFavoriteIndicator.Visibility = model.IS_FAVORITE ? Android.Views.ViewStates.Visible : Android.Views.ViewStates.Invisible;

        }
    }

    public class ReceiveVideoViewHolder : BaseViewHolder, IChatViewHolder<ChatHistoryItemDTO>
    {
        TextView txtTimeStamp;
        ImageView imgFavoriteIndicator;
        public ReceiveVideoViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener) : base(itemView, ClickListener, LongClickListener)
        {
            imgFavoriteIndicator = itemView.FindViewById<ImageView>(Resource.Id.imgFavoriteIndicator);
            txtTimeStamp = itemView.FindViewById<TextView>(Resource.Id.txtTimeStamp);
        }

        public void SetData(ChatHistoryItemDTO model)
        {
            var dateTime = DateConverter.GetDateTimeFromUnixTimeStamp(model.CREATED_AT);
            txtTimeStamp.Text = dateTime.ToString("yyyy-MM-dd hh:mm");
            imgFavoriteIndicator.Visibility = model.IS_FAVORITE ? Android.Views.ViewStates.Visible : Android.Views.ViewStates.Invisible;

        }
    }

    public class ReceiveAudioViewHolder : BaseViewHolder, IChatViewHolder<ChatHistoryItemDTO>
    {
        TextView txtTimeStamp;
        ImageView imgFavoriteIndicator;
        public ReceiveAudioViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener) : base(itemView, ClickListener, LongClickListener)
        {
            imgFavoriteIndicator = itemView.FindViewById<ImageView>(Resource.Id.imgFavoriteIndicator);
            txtTimeStamp = itemView.FindViewById<TextView>(Resource.Id.txtTimeStamp);
        }

        public void SetData(ChatHistoryItemDTO model)
        {
            var dateTime = DateConverter.GetDateTimeFromUnixTimeStamp(model.CREATED_AT);
            txtTimeStamp.Text = dateTime.ToString("yyyy-MM-dd hh:mm");
            imgFavoriteIndicator.Visibility = model.IS_FAVORITE ? Android.Views.ViewStates.Visible : Android.Views.ViewStates.Invisible;

        }
    }

    public class ReceiveImageViewHolder : BaseViewHolder, IChatViewHolder<ChatHistoryItemDTO>
    {
        ImageViewAsync imgContent;
        TextView txtDate;
        ProgressBar pgLoading;
        ImageView imgFavoriteIndicator;

        public ReceiveImageViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener) : base(itemView, ClickListener, LongClickListener)
        {
            imgContent = itemView.FindViewById<ImageViewAsync>(Resource.Id.txtContent);
            txtDate = itemView.FindViewById<TextView>(Resource.Id.txtTimeStamp);
            pgLoading = itemView.FindViewById<ProgressBar>(Resource.Id.pgLoading);
            imgFavoriteIndicator = itemView.FindViewById<ImageView>(Resource.Id.imgFavoriteIndicator);

        }

        public void SetData(ChatHistoryItemDTO model)
        {
            imgFavoriteIndicator.Visibility = model.IS_FAVORITE ? Android.Views.ViewStates.Visible : Android.Views.ViewStates.Invisible;

            if (string.IsNullOrEmpty(model.CONTENT))
            {
                pgLoading.Visibility = Android.Views.ViewStates.Visible;
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
                pgLoading.Visibility = Android.Views.ViewStates.Invisible;

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

   
}
