using System;
namespace WhatMessenger.Model.BaseModel
{
    public class ContactDTO
    {
        public int CONTACT_ID { get; set; }
        public int USERID { get; set; }
        public string NAME { get; set; }
        public string EMAIL { get; set; }
        public string PHONE { get; set; }
        public string PIC { get; set; }
        public bool APPROVE_STATUS { get; set; }
        public bool IS_ACTIVE { get; set; }
        public bool IS_I_BLOCKED { get; set; }
        public bool IS_HE_BLOCKED { get; set; }
        public int USER_UPDATED_AT { get; set; }
        public string USER_STATUS_TITLE { get; set; }
        public int USER_CREATED_DATE { get; set; }
    }
}
