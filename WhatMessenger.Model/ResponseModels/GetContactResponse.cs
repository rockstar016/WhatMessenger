using System.Collections.Generic;
using WhatMessenger.Model.BaseModel;

namespace API.Models.ResponseModels
{
    public class GetContactResponse
    {
        public bool RESULT { get; set; }
        public string MSG { get; set; }
        //public List<ClientContact> CONTACTS { get; set; }
        public int MAX_PAGE { get; set; }
    }
}