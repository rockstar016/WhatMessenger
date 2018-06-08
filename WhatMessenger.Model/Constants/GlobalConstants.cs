using System;
namespace WhatMessenger.Model.Constants
{
    public class GlobalConstants
    {
        public const string RESPONSE_DUPLICATE_ERROR = @"duplicate";
        public const string RESPONSE_SERVER_ERROR = @"server";
        public const string RESPONSE_NO_USER_ERROR = @"nouser";
        public const string RESPONSE_SENT_EMAIL = @"email";
        public const string RESPONSE_BLOCK_ACCOUNT = @"block";
        public const string RESPONSE_SUCCESS = @"success";
        public const string RESPONSE_INVALID_IMEI = @"imei";
        public const string TOKEN_VERIFY_ERROR = @"token";
        public const string INVALID_TOKEN = @"invalid";
        public const int COUNT_RECORDS = 20;


        public const int CHAT_HISTORY_ITEM_TEXT = 0;
        public const int CHAT_HISTORY_ITEM_IMAGE = 1;
        public const int CHAT_HISTORY_ITEM_AUDIO = 2;
        public const int CHAT_HISTORY_ITEM_VIDEO = 3;
        public const int CHAT_HISTORY_ITEM_PDF = 4;
    }
}
