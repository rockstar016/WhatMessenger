using System;
using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using WhatMessenger.Model.BaseModel;
using WhatMessenger.Model.Constants;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.Fragments.Adapters
{
    public class GroupListAdapter: BaseRecycleViewAdapter
    {
        GroupListViewModel ViewModel;
        Activity ParentActivity;
        public GroupListAdapter(Activity ParentActivity, GroupListViewModel ViewModel)
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
            (holder as GroupListItemViewHolder).SetData(ViewModel.Items[position]);
        }

        public override int ItemCount => ViewModel.Items.Count;
    }

    public class GroupListItemViewHolder : RecyclerView.ViewHolder
    {
        ImageViewAsync imgProfile;
        TextView txtName;
        public GroupListItemViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener) : base(itemView)
        {
            itemView.Click += (sender, e) => ClickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => LongClickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            txtName = itemView.FindViewById<TextView>(Resource.Id.txtName);
            imgProfile = itemView.FindViewById<ImageViewAsync>(Resource.Id.imgProfile);
        }

        public void SetData(GroupDTO model)
        {
            txtName.Text = model.GROUP_NAME.Trim();
            if (string.IsNullOrEmpty(model.GROUP_AVATAR))
            {
                ImageService.Instance.LoadCompiledResource("male_placeholder")
                            .Retry(3, 200)
                            .IntoAsync(imgProfile);

            }
            else
            {
                ImageService.Instance.LoadUrl(ServerURL.BaseURL + model.GROUP_AVATAR)
                            .DownSample(width: 180)
                            .Retry(3, 200)
                            .ErrorPlaceholder("male_placeholder")
                            .LoadingPlaceholder("male_placeholder", FFImageLoading.Work.ImageSource.CompiledResource)
                            .Into(imgProfile);
            }
        }
    }

}
