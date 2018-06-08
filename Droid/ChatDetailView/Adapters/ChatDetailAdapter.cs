using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Model.BaseModel;
using WhatMessenger.Model.Constants;
using WhatMessenger.ViewModel;
using WhatMessenger.ViewModel.Utils;

namespace WhatMessenger.Droid.ChatDetailView
{
    public class ChatDetailAdapter : BaseRecycleViewAdapter
    {
        public event EventHandler<RecyclerClickEventArgs> CancelClickHandler;
        ChatDetailView ParentActivity;
        public const int SEND_TEXT = 0;
        public const int SEND_IMAGE = 1;
        public const int SEND_PDF = 2;
        public const int SEND_VIDEO = 3;
        public const int SEND_AUDIO = 4;

        public const int RECEIVE_TEXT = 5;
        public const int RECEIVE_IMAGE = 6;
        public const int RECEIVE_PDF = 7;
        public const int RECEIVE_VIDEO = 8;
        public const int RECEIVE_AUDIO = 9;

        UserDTO Me;
        ChatListViewModel ViewModel;
        public ChatDetailAdapter(ChatDetailView ParentActivity, ChatListViewModel ViewModel, UserDTO Me)
        {
            this.ParentActivity = ParentActivity;
            this.ViewModel = ViewModel;
            this.Me = Me;
        }

        public override int GetItemViewType(int position)
        {
            if(ViewModel.PrivateChatHistoryCollection[position].SENDER_ID == Me.USERID)
            {
                if (ViewModel.PrivateChatHistoryCollection[position].TYPE == GlobalConstants.CHAT_HISTORY_ITEM_TEXT)
                    return SEND_TEXT;
                else if (ViewModel.PrivateChatHistoryCollection[position].TYPE == GlobalConstants.CHAT_HISTORY_ITEM_IMAGE)
                    return SEND_IMAGE;
                else if (ViewModel.PrivateChatHistoryCollection[position].TYPE == GlobalConstants.CHAT_HISTORY_ITEM_PDF)
                    return SEND_PDF;
                else if (ViewModel.PrivateChatHistoryCollection[position].TYPE == GlobalConstants.CHAT_HISTORY_ITEM_VIDEO)
                    return SEND_VIDEO;
                else if (ViewModel.PrivateChatHistoryCollection[position].TYPE == GlobalConstants.CHAT_HISTORY_ITEM_AUDIO)
                    return SEND_AUDIO;
                else
                    return SEND_TEXT;
            }
            else
            {
                if (ViewModel.PrivateChatHistoryCollection[position].TYPE == GlobalConstants.CHAT_HISTORY_ITEM_TEXT)
                    return RECEIVE_TEXT;
                else if (ViewModel.PrivateChatHistoryCollection[position].TYPE == GlobalConstants.CHAT_HISTORY_ITEM_IMAGE)
                    return RECEIVE_IMAGE;
                else if (ViewModel.PrivateChatHistoryCollection[position].TYPE == GlobalConstants.CHAT_HISTORY_ITEM_PDF)
                    return RECEIVE_PDF;
                else if (ViewModel.PrivateChatHistoryCollection[position].TYPE == GlobalConstants.CHAT_HISTORY_ITEM_VIDEO)
                    return RECEIVE_VIDEO;
                else if (ViewModel.PrivateChatHistoryCollection[position].TYPE == GlobalConstants.CHAT_HISTORY_ITEM_AUDIO)
                    return RECEIVE_AUDIO;
                else
                    return RECEIVE_TEXT;
            }
        }

        public override long GetItemId(int position)
        {
            return ViewModel.PrivateChatHistoryCollection[position].ID;
        }

        public override int ItemCount => ViewModel.PrivateChatHistoryCollection.Count;

        public override RecyclerView.ViewHolder OnCreateViewHolder(Android.Views.ViewGroup parent, int viewType)
        {
            if(viewType == SEND_TEXT)
            {
                View itemView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_chat_history_text_send, parent, false);
                return new SendTextViewHolder(itemView, OnClick, OnLongClick);   
            }
            else if(viewType == SEND_IMAGE)
            {
                View itemView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_chat_history_img_send, parent, false);
                return new SendImageViewHolder(itemView, OnClick, OnLongClick, OnCancelClick);   
            }
            else if(viewType == SEND_PDF)
            {
                View itemView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_chat_history_pdf_send, parent, false);
                return new SendPdfViewHolder(itemView, OnClick, OnLongClick, OnCancelClick);   
            }
            else if(viewType == SEND_VIDEO)
            {
                View itemView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_chat_history_video_send, parent, false);
                return new SendVideoViewHolder(itemView, OnClick, OnLongClick, OnCancelClick);   
            }
            else if(viewType == SEND_AUDIO)
            {
                View itemView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_chat_history_voice_send, parent, false);
                return new SendAudioViewHolder(itemView, OnClick, OnLongClick, OnCancelClick);   
            }
            else if(viewType == RECEIVE_TEXT)
            {
                View itemView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_chat_history_text_receive, parent, false);
                return new ReceiveTextViewHolder(itemView, OnClick, OnLongClick);   
            }
            else if(viewType == RECEIVE_PDF)
            {
                View itemView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_chat_history_pdf_receive, parent, false);
                return new ReceivePdfViewHolder(itemView, OnClick, OnLongClick);   
            }
            else if(viewType ==RECEIVE_VIDEO)
            {
                View itemView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_chat_history_video_receive, parent, false);
                return new ReceiveVideoViewHolder(itemView, OnClick, OnLongClick);   
            }
            else if (viewType == RECEIVE_AUDIO)
            {
                View itemView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_chat_history_voice_receive, parent, false);
                return new ReceiveAudioViewHolder(itemView, OnClick, OnLongClick);
            }
            else//receive image
            {
                View itemView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_chat_history_img_receive, parent, false);
                return new ReceiveImageViewHolder(itemView, OnClick, OnLongClick);   
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            (holder as IChatViewHolder<ChatHistoryItemDTO>).SetData(ViewModel.PrivateChatHistoryCollection[position]);
        }


        protected void OnCancelClick(RecyclerClickEventArgs args) => CancelClickHandler?.Invoke(this, args);
    }

    public interface IChatViewHolder<T>
    {
        void SetData(T model);
    }
}
