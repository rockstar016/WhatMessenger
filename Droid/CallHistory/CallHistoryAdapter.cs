using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;

namespace WhatMessenger.Droid.CallHistory
{
    public class CallHistoryAdapter : BaseRecycleViewAdapter
    {
        Activity ParentActivity;
        public CallHistoryAdapter(Activity activity)
        {
            this.ParentActivity = activity;
        }

        public override int GetItemViewType(int position)
        {
            return base.GetItemViewType(position);
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int ItemCount => 3;

        public override Android.Support.V7.Widget.RecyclerView.ViewHolder OnCreateViewHolder(Android.Views.ViewGroup parent, int viewType)
        {
            var rootView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_call_detail_info, parent, false);
            var viewHolder = new CallHistoryViewHolder(rootView);
            return viewHolder;
        }

        public override void OnBindViewHolder(Android.Support.V7.Widget.RecyclerView.ViewHolder holder, int position)
        {
            (holder as CallHistoryViewHolder).SetDataProvider();
        }
    }

    public class CallHistoryViewHolder : RecyclerView.ViewHolder
    {
        CallDialyHistoryView historyLinearLayout;
        public CallHistoryViewHolder(View itemview) : base(itemview)
        {
            historyLinearLayout = itemview.FindViewById<CallDialyHistoryView>(Resource.Id.containerHistoryLayout);
        }

        public void SetDataProvider()
        {
            var HistoryList = new List<string>();
            HistoryList.Add("");
            HistoryList.Add("");
            HistoryList.Add("");
            historyLinearLayout.SetDataProvider(HistoryList);
        }
    }
}
