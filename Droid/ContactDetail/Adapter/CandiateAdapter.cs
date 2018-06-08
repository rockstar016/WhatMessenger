using System;
using System.Collections.Generic;
using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using FFImageLoading.Work;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Model.BaseModel;
using WhatMessenger.Model.Constants;
using WhatMessenger.ViewModel;

namespace WhatMessenger.Droid.ContactDetail.Adapter
{
    public class CandiateAdapter : BaseRecycleViewAdapter
    {
        Activity ParentActivity;
        ContactListViewModel ViewModel;
        public CandiateAdapter(Activity activity, ContactListViewModel ViewModel)
        {
            this.ParentActivity = activity;
            this.ViewModel = ViewModel;
            ViewModel.CandiateList.CollectionChanged += (sender, args) =>
            {
                if(ParentActivity != null)
                this.ParentActivity.RunOnUiThread(NotifyDataSetChanged);
            };
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int ItemCount => ViewModel.CandiateList.Count;

        public override Android.Support.V7.Widget.RecyclerView.ViewHolder OnCreateViewHolder(Android.Views.ViewGroup parent, int viewType)
        {
            var rootView = LayoutInflater.From(ParentActivity).Inflate(Resource.Layout.item_contact_candidate, parent, false);
            var viewHolder = new ContactCandidateViewHolder(rootView, OnClick, OnLongClick);
            return viewHolder;
        }

        public override void OnBindViewHolder(Android.Support.V7.Widget.RecyclerView.ViewHolder holder, int position)
        {
            (holder as ContactCandidateViewHolder).SetDataProvider(ViewModel.CandiateList[position]);
        }
    }


    public class ContactCandidateViewHolder : BaseViewHolder
    {
        ImageViewAsync imgView;
        TextView txtName, txtDescription;
        Button btAddContact;
        Action<RecyclerClickEventArgs> ClickListener, LongClickListener;
        View rootView;
        public ContactCandidateViewHolder(View itemview, Action<RecyclerClickEventArgs> ClickListener, Action<RecyclerClickEventArgs> LongClickListener) : base(itemview, ClickListener, LongClickListener)
        {
            this.rootView = itemview;
            this.imgView = itemview.FindViewById<ImageViewAsync>(Resource.Id.imgProfile);
            this.txtName = itemview.FindViewById<TextView>(Resource.Id.txtName);
            this.txtDescription = itemview.FindViewById<TextView>(Resource.Id.txtDescription);
            this.btAddContact = itemview.FindViewById<Button>(Resource.Id.btAddContact);

            this.ClickListener = ClickListener;
            this.LongClickListener = LongClickListener;

        }

        public void SetDataProvider(ContactDTO model)
        {
            btAddContact.Enabled = true;
            btAddContact.Text = @"Add";
            txtName.Text = model.NAME.Trim();
            txtDescription.Text = @"Email: " + model.EMAIL.Trim();
            if(string.IsNullOrEmpty(model.PIC))
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

            btAddContact.Click += (sender, e) => {
                btAddContact.Enabled = false;
                btAddContact.Text = @"Sent";
                ClickListener(new RecyclerClickEventArgs { View = rootView, Position = AdapterPosition });
            };

            rootView.Click += (sender, e) => {
                ClickListener(new RecyclerClickEventArgs { View = rootView, Position = AdapterPosition });
            };

        }
    }
}
