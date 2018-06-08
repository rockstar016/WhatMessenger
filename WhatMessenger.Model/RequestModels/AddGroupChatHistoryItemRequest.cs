using System;
namespace WhatMessenger.Model.RequestModels
{
    public class AddGroupChatHistoryItemRequest
    {
        public int SENDER_ID { get; set; }
        public string SENDER_NAME { get; set; }
        public int GROUP_ID { get; set; }
        /**
         * 0: TEXT, 1: IMAGE, 2: AUDIO, 3: VIDEO
         */
        public int MSG_TYPE { get; set; }
        public string MSG_CONTENT { get; set; }
    }

    public class AddGroupVoiceMessageItemRequest
    {
        public AddGroupChatHistoryItemRequest ItemRequest { get; set; }
        public string FileName { get; set; }
        public byte[] FilebArray { get; set; }
    }
}
