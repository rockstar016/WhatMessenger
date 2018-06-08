using System;
using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.Fragments.Adapters
{
    public class FavoriteListAdapter : BaseRecycleViewAdapter
    {
        FavoriteViewModel ViewModel;
        Activity ParentActivity;
        public FavoriteListAdapter(Activity ParentActivity, FavoriteViewModel ViewModel)
        {
            this.ParentActivity = ParentActivity;
            this.ViewModel = ViewModel;

            ViewModel.Items.CollectionChanged += (sender, args) =>
            {
                this.ParentActivity.RunOnUiThread(NotifyDataSetChanged);
            };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(Android.Views.ViewGroup parent, int viewType)
        {
            var itemView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_group_history, parent, false);
            var holder = new GroupListItemViewHolder(itemView, OnClick, OnLongClick);
            return holder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {

        }


        public override int ItemCount => ViewModel.Items.Count;
    }
}
