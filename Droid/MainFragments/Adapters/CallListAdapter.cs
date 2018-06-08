using System;
using System.Collections.Specialized;
using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Droid.CallHistory;
using WhatMessenger.Model;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.Fragments.Adapters
{
    public class CallListAdapter: BaseRecycleViewAdapter
    {
        CallListViewModel ViewModel;
        Activity ParentActivity;
        public CallListAdapter(Activity ParentActivity, CallListViewModel ViewModel)
        {
            this.ParentActivity = ParentActivity;
            this.ViewModel = ViewModel;

            ViewModel.Items.CollectionChanged += (sender, args) =>
            {
                if (args.Action == NotifyCollectionChangedAction.Reset)
                    this.ParentActivity.RunOnUiThread(NotifyDataSetChanged);

            };


        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(Android.Views.ViewGroup parent, int viewType)
        {

            var itemView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_call_history, parent, false);
            var holder = new CallHistoryItemViewHolder(itemView, OnClick, OnLongClick);
            return holder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
        }

        public override int ItemCount => ViewModel.Items.Count;

    }

    public class CallHistoryItemViewHolder : BaseViewHolder
    {
        
        public CallHistoryItemViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener) : base(itemView, ClickListener, LongClickListener)
        {

        }

        public void InitDataView(CallListItem ItemModel)
        {
              
        }

    }
}
