using System;
using Android.Support.V7.Widget;
using Android.Views;

namespace WhatMessenger.Droid.Bases
{
    public class BaseViewHolder : RecyclerView.ViewHolder
    {
        public BaseViewHolder(View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener):base(itemView)
        {
            itemView.Click += (sender, e) => ClickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => LongClickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }
}
