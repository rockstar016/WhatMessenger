
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using WhatMessenger.Model.BaseModel;
using WhatMessenger.Model.Constants;
using WhatMessenger.ViewModel;
using WhatMessenger.ViewModel.Utils;

namespace WhatMessenger.Droid.ChatDetailView
{
    [Activity(Label = "GroupDetailAdapter")]
    public class GroupDetailAdapter  : BaseRecycleViewAdapter
    {
        public event EventHandler<RecyclerClickEventArgs> CancelClickHandler;

        GroupChatDetailView ParentActivity;
        public const int SEND_TEXT = 0;
        public const int SEND_IMAGE = 1;
        public const int SEND_VIDEO = 2;
        public const int SEND_AUDIO = 3;
        public const int SEND_PDF = 4;

        public const int RECEIVE_TEXT = 5;
        public const int RECEIVE_IMAGE = 6;
        public const int RECEIVE_VIDEO = 7;
        public const int RECEIVE_AUDIO = 8;
        public const int RECEIVE_PDF = 9;
        UserDTO Me;
        GroupListViewModel ViewModel;

        public GroupDetailAdapter(GroupChatDetailView ParentActivity, GroupListViewModel ViewModel, UserDTO Me)
        {
            this.ParentActivity = ParentActivity;
            this.ViewModel = ViewModel;
            this.Me = Me;
        }

        public override int GetItemViewType(int position)
        {
            if (ViewModel.GroupChatHistoryItemList[position].SENDER_ID == Me.USERID)
            {
                if (ViewModel.GroupChatHistoryItemList[position].TYPE == GlobalConstants.CHAT_HISTORY_ITEM_TEXT)
                    return SEND_TEXT;
                else if (ViewModel.GroupChatHistoryItemList[position].TYPE == GlobalConstants.CHAT_HISTORY_ITEM_IMAGE)
                    return SEND_IMAGE;
                else if (ViewModel.GroupChatHistoryItemList[position].TYPE == GlobalConstants.CHAT_HISTORY_ITEM_VIDEO)
                    return SEND_VIDEO;
                else if (ViewModel.GroupChatHistoryItemList[position].TYPE == GlobalConstants.CHAT_HISTORY_ITEM_AUDIO)
                    return SEND_AUDIO;
                else if (ViewModel.GroupChatHistoryItemList[position].TYPE == GlobalConstants.CHAT_HISTORY_ITEM_PDF)
                    return SEND_PDF;
                else
                    return SEND_TEXT;
            }
            else
            {
                if (ViewModel.GroupChatHistoryItemList[position].TYPE == GlobalConstants.CHAT_HISTORY_ITEM_TEXT)
                    return RECEIVE_TEXT;
                else if (ViewModel.GroupChatHistoryItemList[position].TYPE == GlobalConstants.CHAT_HISTORY_ITEM_IMAGE)
                    return RECEIVE_IMAGE;
                else if (ViewModel.GroupChatHistoryItemList[position].TYPE == GlobalConstants.CHAT_HISTORY_ITEM_AUDIO)
                    return RECEIVE_AUDIO;
                else if (ViewModel.GroupChatHistoryItemList[position].TYPE == GlobalConstants.CHAT_HISTORY_ITEM_VIDEO)
                    return RECEIVE_VIDEO;
                else if (ViewModel.GroupChatHistoryItemList[position].TYPE == GlobalConstants.CHAT_HISTORY_ITEM_PDF)
                    return RECEIVE_PDF;
                else
                    return RECEIVE_TEXT;
            }
        }

        public override long GetItemId(int position)
        {
            return ViewModel.GroupChatHistoryItemList[position].ID;
        }

        public override int ItemCount => ViewModel.GroupChatHistoryItemList.Count;

        public override RecyclerView.ViewHolder OnCreateViewHolder(Android.Views.ViewGroup parent, int viewType)
        {
            if (viewType == SEND_TEXT)
            {
                View itemView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_chat_history_text_send, parent, false);
                return new GroupSendTextViewHolder(itemView, OnClick, OnLongClick);
            }
            else if (viewType == SEND_IMAGE)
            {
                View itemView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_chat_history_img_send, parent, false);
                return new GroupSendImageViewHolder(itemView, OnClick, OnLongClick, OnCancelClick);
            }
            else if(viewType == SEND_VIDEO)
            {
                View itemView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_chat_history_video_send, parent, false);
                return new GroupSendVideoViewHolder(itemView, OnClick, OnLongClick, OnCancelClick);
            }
            else if(viewType == SEND_AUDIO)
            {
                View itemView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_chat_history_voice_send, parent, false);
                return new GroupSendAudioViewHolder(itemView, OnClick, OnLongClick, OnCancelClick);
            }
            else if(viewType == SEND_PDF)
            {
                View itemView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_chat_history_pdf_send, parent, false);
                return new GroupSendPdfViewHolder(itemView, OnClick, OnLongClick, OnCancelClick);
            }
            else if (viewType == RECEIVE_TEXT)
            {
                View itemView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_group_history_text_receive, parent, false);
                return new GroupReceiveTextViewHolder(itemView, OnClick, OnLongClick);
            }
            else if (viewType == RECEIVE_IMAGE)
            {
                View itemView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_group_history_img_receive, parent, false);
                return new GroupReceiveImageViewHolder(itemView, OnClick, OnLongClick);
            }
            else if (viewType == RECEIVE_VIDEO)
            {
                View itemView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_group_history_video_receive, parent, false);
                return new GroupReceiveVideoViewHolder(itemView, OnClick, OnLongClick);
            }
            else if (viewType == RECEIVE_AUDIO)
            {
                View itemView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_group_history_voice_receive, parent, false);
                return new GroupReceiveAudioViewHolder(itemView, OnClick, OnLongClick);
            }
            else if (viewType == RECEIVE_PDF)
            {
                View itemView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_group_history_pdf_receive, parent, false);
                return new GroupReceivePdfViewHolder(itemView, OnClick, OnLongClick);
            }
            else//receive text
            {
                View itemView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_group_history_text_receive, parent, false);
                return new GroupReceiveImageViewHolder(itemView, OnClick, OnLongClick);
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            (holder as IChatViewHolder<GroupHistoryItemDTO>).SetData(ViewModel.GroupChatHistoryItemList[position]);
        }

        protected void OnCancelClick(RecyclerClickEventArgs args) => CancelClickHandler?.Invoke(this, args);
    }
}
