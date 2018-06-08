using System;
namespace WhatMessenger.Model.RequestModels
{
    public class OpenOrCreateChatEntryRequest
    {
        public string ConnectionId { get; set; }
        public int MYID { get; set; }
        public int OTHERID { get; set; }
    }
}
