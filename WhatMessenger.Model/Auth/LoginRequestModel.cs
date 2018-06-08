using System;
namespace WhatMessenger.Model.Auth
{
    public class LoginRequestModel
    {
        public string EMAIL { get; set; }
        public string PASSWORD { get; set; }
        public string IMEI { get; set; }
    }
}
