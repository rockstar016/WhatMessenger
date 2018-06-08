using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace WhatMessenger.Droid.CallHistory
{
    public class CallDialyHistoryView : LinearLayout
    {
        
        public CallDialyHistoryView(Android.Content.Context context) : base(context)
        {
        }

        public CallDialyHistoryView(Android.Content.Context context, Android.Util.IAttributeSet attrs) : base(context, attrs)
        {
        }

        public CallDialyHistoryView(Android.Content.Context context, Android.Util.IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public CallDialyHistoryView(Android.Content.Context context, Android.Util.IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        public void SetDataProvider(List<string> DataProvider)
        {
            for (int i = 0; i < DataProvider.Count; i++){
                var itemView = LayoutInflater.From(Context).Inflate(Resource.Layout.item_call_history_item_view, this, false);
                this.AddView(itemView);    
            }
        }
    }
}
