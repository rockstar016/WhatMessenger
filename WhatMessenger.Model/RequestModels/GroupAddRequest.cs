using System;
using WhatMessenger.Model.BaseModel;

namespace WhatMessenger.Model.RequestModels
{
    public class GroupAddRequest
    {
        public string TOKEN { get; set; }
        public int MY_USER_ID { get; set; }
        public string GROUP_NAME { get; set; }
        public string OWNER_NAME { get; set; }
    }
}
