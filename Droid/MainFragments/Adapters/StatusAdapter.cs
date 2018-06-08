using System;
using System.Collections.Generic;
using Android.App;
using Android.Graphics;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Refractored.Controls;
using Square.Picasso;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Model.BaseModel;
using WhatMessenger.Model.Constants;
using WhatMessenger.ViewModel;
using static Android.Support.V7.Widget.RecyclerView;

namespace WhatMessenger.Droid.Fragments.Adapters
{
    public class StatusAdapter : BaseRecycleViewAdapter
    {
        Activity ParentActivity;
        ProfileViewModel ViewModel { get; set; }
        List<string> StatusTitleArray;

        public StatusAdapter(Activity parentActivity, ProfileViewModel ViewModel, List<string> StatusTitleList)
        {
            this.ViewModel = ViewModel;
            this.ParentActivity = parentActivity;
            this.StatusTitleArray = StatusTitleList;
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName.Equals(nameof(ProfileViewModel.ME )))
            {
                ParentActivity.RunOnUiThread(NotifyDataSetChanged);
            }
        }

        public override int ItemCount => StatusTitleArray.Count;

        public override RecyclerView.ViewHolder OnCreateViewHolder(Android.Views.ViewGroup parent, int viewType)
        {
            var itemView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_status_kind_viewholder, parent, false);
            var holder = new StatusViewHolder(itemView, OnClick, OnLongClick);
            return holder;
        }

        public override void OnBindViewHolder(ViewHolder holder, int position)
        {
            (holder as StatusViewHolder).SetData(StatusTitleArray[position], ViewModel.ME);
        }
    }

    public class StatusViewHolder:BaseViewHolder
    {
        Button btStatusKind;
        public StatusViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener) : base(itemView, ClickListener, LongClickListener)
        {
            btStatusKind = itemView.FindViewById<Button>(Resource.Id.btStatusKind);
            btStatusKind.Click += (sender, e) =>  ClickListener(new RecyclerClickEventArgs() { Position = AdapterPosition, View = btStatusKind });
        }

        public void SetData(string title, UserDTO MeDTO)
        {
            btStatusKind.Text = title;
            if(string.Equals(MeDTO.STATUS_INDICATOR.Trim(), title))
            {
                btStatusKind.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(btStatusKind.Context, Resource.Color.colorPrimary)));
            }
            else
            {
                btStatusKind.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(btStatusKind.Context, Resource.Color.secondaryTextColor)));
            }
        }
    }
}
