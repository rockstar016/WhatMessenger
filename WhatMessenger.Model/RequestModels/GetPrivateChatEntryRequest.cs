using System;
namespace WhatMessenger.Model.RequestModels
{
    public class GetPrivateChatEntryRequest
    {
        public string TOKEN { get; set; }
        public int MY_USER_ID { get; set; }
    }
}
