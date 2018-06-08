using System;
using System.Collections.Generic;
using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Model.BaseModel;
using WhatMessenger.Model.Constants;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.GroupDetail.Adapters
{
    public class GroupEditAdapter:BaseRecycleViewAdapter
    {
        Activity Activity;
        public List<UserDTO> Contactors { get; set; }
        public GroupEditAdapter(Activity Activity, List<UserDTO> Contactors)
        {
            this.Contactors = Contactors;
            this.Activity = Activity;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(Android.Views.ViewGroup parent, int viewType)
        {
            var itemView = LayoutInflater.From(this.Activity).Inflate(Resource.Layout.item_group_member_edit, parent, false);
            var holder = new GroupEditorItemViewHolder(itemView, OnClick, OnLongClick);
            return holder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            (holder as GroupEditorItemViewHolder).InitDataView(Contactors[position]);
        }

        public override int ItemCount => Contactors.Count;
    }


    public class GroupEditorItemViewHolder : BaseViewHolder
    {
        ImageViewAsync imgProfile { get; set; }
        TextView txtName, txtDescription;
        public GroupEditorItemViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener) : base(itemView, ClickListener, LongClickListener)
        {
            txtName = itemView.FindViewById<TextView>(Resource.Id.txtName);
            txtDescription = itemView.FindViewById<TextView>(Resource.Id.txtDescription);
            imgProfile = itemView.FindViewById<ImageViewAsync>(Resource.Id.imgProfile);
        }

        public void InitDataView(UserDTO ItemModel)
        {
            txtName.Text = ItemModel.NAME;
            txtDescription.Text = ItemModel.STATUS_INDICATOR;

            if (string.IsNullOrEmpty(ItemModel.PHOTO))
            {
                ImageService.Instance.LoadCompiledResource("female_placeholder")
                            .Retry(3, 200)
                            .IntoAsync(imgProfile);

            }
            else
            {
                ImageService.Instance.LoadUrl(ServerURL.BaseURL + ItemModel.PHOTO)
                            .DownSample(width: 150)
                            .Retry(3, 200)
                            .ErrorPlaceholder("female_placeholder")
                            .Into(imgProfile);
            }
        }
    }


    public class CustomLinearLayoutManager : LinearLayoutManager
    {
        private bool isScrollEnabled = true;
        public CustomLinearLayoutManager(IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public CustomLinearLayoutManager(Android.Content.Context context) : base(context)
        {
        }

        public CustomLinearLayoutManager(Android.Content.Context context, Android.Util.IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        public CustomLinearLayoutManager(Android.Content.Context context, int orientation, bool reverseLayout) : base(context, orientation, reverseLayout)
        {
        }

        public void setScrollEnabled(bool flag)
        {
            isScrollEnabled = flag;
        }

        public override bool CanScrollVertically()
        {
            return isScrollEnabled && base.CanScrollVertically();
        }
    }
}
