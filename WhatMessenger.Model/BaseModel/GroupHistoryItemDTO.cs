using System;
namespace WhatMessenger.Model.BaseModel
{
    public class GroupHistoryItemDTO
    {
        public int ID { get; set; }
        public int THREAD_ID { get; set; }
        public string SENDER_NAME { get; set; }
        public int SENDER_ID { get; set; }
        public string CONTENT { get; set; }
        /**
         * 0: TEXT, 1: IMAGE, 2: AUDIO, 3: VIDEO, 4: PDF
         */
        public int TYPE { get; set; }
        public bool READ_STATUS { get; set; }
        public int CREATED_AT { get; set; }
    }
}
