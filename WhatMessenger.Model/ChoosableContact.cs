using System;
using WhatMessenger.Model.BaseModel;

namespace WhatMessenger.Model
{
    public class ChoosableContact
    {
        public ContactDTO ContactDTO { get; set; }
        public bool Choose { get; set; }
    }
}
