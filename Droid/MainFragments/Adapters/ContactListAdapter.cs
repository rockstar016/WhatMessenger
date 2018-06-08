using System;
using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Model.BaseModel;
using WhatMessenger.Model.Constants;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.Fragments.Adapters
{
    public class ContactListAdapter: BaseRecycleViewAdapter
    {
        ContactListViewModel ViewModel;
        Activity ParentActivity;
        public ContactListAdapter(Activity ParentActivity, ContactListViewModel ViewModel)
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

            var itemView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_contact_history, parent, false);
            var holder = new ContactListItemViewHolder(itemView, OnClick, OnLongClick);
            return holder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            (holder as ContactListItemViewHolder).SetData(ViewModel.Items[position]);
        }


        public override int ItemCount => ViewModel.Items.Count;

    }

    public class ContactListItemViewHolder : BaseViewHolder
    {
        ImageViewAsync imgView;
        TextView txtName,txtEmail;
        public ContactListItemViewHolder(Android.Views.View itemView, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener) : base(itemView, ClickListener, LongClickListener)
        {
            imgView = itemView.FindViewById<ImageViewAsync>(Resource.Id.imgProfile);
            txtName = itemView.FindViewById<TextView>(Resource.Id.txtName);
            txtEmail = itemView.FindViewById<TextView>(Resource.Id.txtEmail);
        }


        public void SetData(ContactDTO model)
        {
            if (string.IsNullOrEmpty(model.PIC))
            {
                ImageService.Instance.LoadCompiledResource("female_placeholder")
                            .Retry(3, 200)
                            .IntoAsync(imgView);

            }
            else
            {
                ImageService.Instance.LoadUrl(ServerURL.BaseURL + model.PIC)
                            .DownSample(width: 180)
                            .Retry(3, 200)
                            .Into(imgView);
            }
            txtName.Text = model.NAME.Trim();
            txtEmail.Text = model.EMAIL.Trim();
        }
    }

}
