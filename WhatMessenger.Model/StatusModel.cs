using System;
namespace WhatMessenger.Model
{
    public class StatusModel
    {
        public string Id { get; set; }

        public bool NewStatus { get; set; }
        public string ImagePath { get; set; }
        public string CoverText { get; set; }
        //public int ViewedUserCount { get; set; }
    }
}
