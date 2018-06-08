using System;
namespace WhatMessenger.Model.RequestModels
{
    public class LoadChatHistoryCollectionRequest
    {
        public string TOKEN { get; set; }
        public int THREAD_ID { get; set; }
        public int LAST_MESSAGE_ID { get; set; }
    }
}
