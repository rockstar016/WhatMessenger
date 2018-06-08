using System;
using System.Collections.Generic;

namespace WhatMessenger.Model.BaseModel
{
    public class GroupDTO
    {
        public int GROUP_ID { get; set; }
        public int CREATED_AT { get; set; }
        public string GROUP_NAME { get; set; }
        public string GROUP_AVATAR { get; set; }
        public int GROUP_OWNER_ID { get; set; }
        public string GROUP_OWNER_NAME { get; set; }
        public List<UserDTO> MEMBER_LIST { get; set; }
    }
}
