using System;
namespace WhatMessenger.Model.Chat
{
    public class TypingStatus
    {
        public int ThreadId { get; set; }
        public int MyUserId { get; set; }
        public int OtherUserId { get; set; }
        public bool IsTyping { get; set; } 
    }
}
