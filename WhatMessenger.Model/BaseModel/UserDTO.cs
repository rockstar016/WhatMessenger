using System;
namespace WhatMessenger.Model.BaseModel
{
    public class UserDTO
    {
        //todo insert userlang id, too
        public int USERID { get; set; }
        public string TOKEN { get; set; }
        public string EMAIL { get; set; }
        public string PHONE { get; set; }
        public string NAME { get; set; }
        public string PHOTO { get; set; }
        public bool IS_ACTIVE { get; set; }
        public int CREATED_AT { get; set; }
        public int LANG { get; set; }//1: chinese, 0: english
        public int SHOW_PROFILE_TO { get; set; }
        public int SHOW_STATUS_TO { get; set; }
        public string STATUS_INDICATOR { get; set; }
    }
}
