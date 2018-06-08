using System;
using WhatMessenger.Model.BaseModel;

namespace WhatMessenger.Droid.Helpers
{
    public class RecyclerContactEventArgs: EventArgs
    {
        public ContactDTO DTO { get; set; }
    }
}
