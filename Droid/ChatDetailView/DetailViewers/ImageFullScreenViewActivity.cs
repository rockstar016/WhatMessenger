
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ImageViews.Photo;
using Newtonsoft.Json;
using Square.Picasso;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Model.BaseModel;
using WhatMessenger.Model.Constants;

namespace WhatMessenger.Droid.ChatDetailView
{
    [Activity(Label = "ImageFullScreenViewActivity")]
    public class ImageFullScreenViewActivity : BaseActivity
    {
        public const string FILE_PATH = "FILE_PATH";

        protected override int LayoutResource => Resource.Layout.activity_full_image;
        ImageButton btClose;
        PhotoView imgContent;
        TextView txtName;
        string FilePath;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            btClose = FindViewById<ImageButton>(Resource.Id.btClose);
            btClose.Click += (sender, e) => Finish();
            imgContent = FindViewById<PhotoView>(Resource.Id.imgPic);
            txtName = FindViewById<TextView>(Resource.Id.txtName);
            txtName.SetTextColor(Android.Graphics.Color.White);
            FilePath = Intent.GetStringExtra(ImageFullScreenViewActivity.FILE_PATH);
            InitViews();
        }

        void InitViews()
        {
            txtName.Text = Path.GetFileName(FilePath);
            ShowLoadingDialog("Loading");
            Picasso.With(this)
                   .Load(ServerURL.BaseURL + FilePath)
                   .Error(Resource.Drawable.placeholder_img_thumb)
                   .Into(imgContent, 
                         () => {
                RunOnUiThread(() => {HideLoadingDialog();}); }, 
                         () => { RunOnUiThread(() => { HideLoadingDialog(); }); });
            
        }
    }
}
