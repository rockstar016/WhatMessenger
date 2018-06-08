using System;
namespace WhatMessenger.Model.RequestModels
{
    public class GetGroupChatEntryRequest
    {
        public int GROUP_CHAT_ID { get; set; }
        public int MY_USER_ID { get; set; }
    }
}
