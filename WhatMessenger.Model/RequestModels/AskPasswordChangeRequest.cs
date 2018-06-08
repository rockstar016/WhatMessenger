using System;
namespace WhatMessenger.Model.RequestModels
{
    public class AskPasswordChangeRequest
    {
        public string IMEI { get; set; }
        public string EMAIL { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string IMEI { get; set; }
        public string CODE { get; set; }
        public string NEW_PASS { get; set; }
    }
}
