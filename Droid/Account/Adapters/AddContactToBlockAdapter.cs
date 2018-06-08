using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Droid.Helpers;
using WhatMessenger.Model.BaseModel;
using WhatMessenger.Model.Constants;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.Account.Adapters
{
    public class AddContactToBlockAdapter: BaseRecycleViewAdapter
    {
        Activity ParentActivity;
        ContactListViewModel ViewModel;
        List<ContactDTO> dataProvider;
        public AddContactToBlockAdapter(Activity activity, ContactListViewModel ViewModel)
        {
            this.ParentActivity = activity;
            this.ViewModel = ViewModel;
            dataProvider = ViewModel.Items.Where(u => u.IS_I_BLOCKED == false).ToList();
            ViewModel.Items.CollectionChanged += (sender, args) =>
            {
                dataProvider = ViewModel.Items.Where(u => u.IS_I_BLOCKED == false).ToList();
                if (ParentActivity != null)
                    this.ParentActivity.RunOnUiThread(NotifyDataSetChanged);
            };
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int ItemCount => dataProvider.Count;

        public override Android.Support.V7.Widget.RecyclerView.ViewHolder OnCreateViewHolder(Android.Views.ViewGroup parent, int viewType)
        {
            var rootView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_contact_candidate, parent, false);
            var viewHolder = new AvailableContactViewHolder(rootView, OnClick, OnLongClick);
            return viewHolder;
        }

        public override void OnBindViewHolder(Android.Support.V7.Widget.RecyclerView.ViewHolder holder, int position)
        {
            (holder as AvailableContactViewHolder).SetDataProvider(dataProvider[position]);
        }
    }

    public class AvailableContactViewHolder : BaseViewHolder
    {
        ImageViewAsync imgView;
        TextView txtName, txtDescription;
        Button btAddContact;

        View rootView;
        public AvailableContactViewHolder(View itemview, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener) : base(itemview, ClickListener, LongClickListener)
        {
            this.rootView = itemview;
            this.imgView = itemview.FindViewById<ImageViewAsync>(Resource.Id.imgProfile);
            this.txtName = itemview.FindViewById<TextView>(Resource.Id.txtName);
            this.txtDescription = itemview.FindViewById<TextView>(Resource.Id.txtDescription);
            this.btAddContact = itemview.FindViewById<Button>(Resource.Id.btAddContact);

            btAddContact.Click += (sender, e) => {
                btAddContact.Enabled = false;
                btAddContact.Text = @"Sent";
                ClickListener(new RecyclerClickEventArgs { View = btAddContact, Position = AdapterPosition });
            };
        }

        public void SetDataProvider(ContactDTO model)
        {
            btAddContact.Enabled = true;
            btAddContact.Text = @"Block";
            txtName.Text = model.NAME.Trim();
            txtDescription.Text = @"Email: " + model.EMAIL.Trim();
            if (string.IsNullOrEmpty(model.PIC))
            {
                ImageService.Instance.LoadCompiledResource("female_placeholder")
                            .Retry(3, 200)
                            .DownSample(90, 90)
                            .Transform(new CircleTransformation())
                            .IntoAsync(imgView);

            }
            else
            {
                ImageService.Instance.LoadUrl(ServerURL.BaseURL + model.PIC)
                            .Retry(3, 200)
                            .DownSample(90, 90)
                            .Transform(new CircleTransformation())
                            .Into(imgView);
            }



        }
    }

}
