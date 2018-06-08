using System;
namespace WhatMessenger.Model.RequestModels
{
    public class OnlineStatusRequest
    {
        public string UserId { get; set; }
        public bool IsOnline { get; set; }
    }
}
