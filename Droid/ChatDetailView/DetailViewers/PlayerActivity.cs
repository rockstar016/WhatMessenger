
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
using Android.Webkit;
using Android.Widget;
using ImageViews.Photo;
using Newtonsoft.Json;
using Square.Picasso;
using WhatMessenger.Droid.Bases;
using WhatMessenger.Model.BaseModel;
using WhatMessenger.Model.Constants;
using static Android.Provider.SyncStateContract;

namespace WhatMessenger.Droid.ChatDetailView
{
    [Activity(Label = "ImageFullScreenViewActivity")]
    public class PlayerActivity : BaseActivity
    {
        public const string BUNDLE_VIDEO_URL = "VIDEO_URL";
        protected override int LayoutResource => Resource.Layout.activity_video_player;
        VideoView videoPlayer;
        string VIDEO_URL;
        TextView txtName;
        ImageButton btClose;
        ProgressBar pgLoading;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            VIDEO_URL = Intent.GetStringExtra(BUNDLE_VIDEO_URL);
            videoPlayer = FindViewById<VideoView>(Resource.Id.videoPlayer);
            txtName = FindViewById<TextView>(Resource.Id.txtName);
            btClose = FindViewById<ImageButton>(Resource.Id.btClose);
            pgLoading = FindViewById<ProgressBar>(Resource.Id.pgLoading);
            txtName.Text = Path.GetFileName(VIDEO_URL);
            btClose.Click += BtClose_Click;
            Android.Net.Uri url = Android.Net.Uri.Parse(VIDEO_URL);
            MediaController controller = new MediaController(this);
            controller.SetAnchorView(videoPlayer);
            videoPlayer.SetVideoURI(url);
            videoPlayer.SetMediaController(controller);
            videoPlayer.Prepared += VideoPlayer_Prepared;
        }

        void VideoPlayer_Prepared(object sender, EventArgs e)
        {
            pgLoading.Visibility = ViewStates.Invisible;
            videoPlayer.Start();
        }

        void BtClose_Click(object sender, EventArgs e)
        {
            Finish();
        }

    }
}
