using System;
namespace WhatMessenger.Model.BaseModel
{
    public class ChatEntryDTO
    {
        public int EntryID { get; set; }
        public int MyUserId { get; set; }
        public int OtherUserId { get; set; }
        public string OtherUserName { get; set; }
        public string LastMessage { get; set; }
        public bool OtherUserOnlineStatus { get; set; }
        public bool OtherUserActivate { get; set; }
        public bool IsBlocked { get; set; }
        public int UnreadMessageCount { get; set; }
        public int LastUpdateDate { get; set; }
        public string OtherUserPic { get; set; }
    }
}
