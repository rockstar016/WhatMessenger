using System;
namespace WhatMessenger.Model.RequestModels
{
    public class AddPrivateChatHistoryItemRequest
    {
        public int THREAD_ID { get; set; }
        public int SENDER_ID { get; set; }
        public int OTHER_USER_ID { get; set; }
        /**
         * 0: TEXT, 1: IMAGE, 2: AUDIO, 3: VIDEO, 4: FILE
         */
        public int MSG_TYPE { get; set; }
        public string MSG_CONTENT { get; set; }
    }

    public class AddPrivateVoiceMessageItemRequest
    {
        public AddPrivateChatHistoryItemRequest ItemRequest { get; set; }
        public string FileName { get; set; }
        public byte[] FilebArray { get; set; }
    }
}
