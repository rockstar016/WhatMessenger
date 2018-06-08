
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace WhatMessenger.Droid.ChatDetailView
{
    public class AttachToolboxStatus : Android.Support.V4.App.Fragment
    {
        public static AttachToolboxStatus GetInstance() => new AttachToolboxStatus { Arguments = new Bundle() };

        public event EventHandler ImageChooseHandler, VideoChooseHandler, ImageTakeHandler, VideoTakeHandler, AttachFileHandler;

        ImageButton btChooseImage, btChooseVideo, btTakeImage, btTakeVideo, btAttachFile;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.fragment_attach_toolbox, container, false);
            btTakeImage = rootView.FindViewById<ImageButton>(Resource.Id.btTakeImage);
            btTakeVideo = rootView.FindViewById<ImageButton>(Resource.Id.btTakeVideo);
            btChooseImage = rootView.FindViewById<ImageButton>(Resource.Id.btReadImage);
            btChooseVideo = rootView.FindViewById<ImageButton>(Resource.Id.btReadVideo);
            btAttachFile = rootView.FindViewById<ImageButton>(Resource.Id.btAttachFile);
            btTakeImage.Click += (sender, e) => { 
                ImageTakeHandler.Invoke(null, null); 
            };
            btTakeVideo.Click += (sender, e) => { 
                VideoTakeHandler.Invoke(null, null); 
            };
            btChooseImage.Click += (sender, e) => { 
                ImageChooseHandler.Invoke(null, null); 
            };
            btChooseVideo.Click += (sender, e) => { 
                VideoChooseHandler.Invoke(null, null); 
            };
            btAttachFile.Click += (sender, e) =>
            {
                AttachFileHandler.Invoke(null, null);
            };
            return rootView;
        }
    }
}
