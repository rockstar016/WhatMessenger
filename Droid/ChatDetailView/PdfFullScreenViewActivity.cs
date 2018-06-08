
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
    public class PdfFullScreenViewActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_pdf_viewer;
        WebView imgPic;
        ChatHistoryItemDTO DTO;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            imgPic = FindViewById<WebView>(Resource.Id.imgPic);
            var ChatDTOStr = Intent.GetStringExtra("DTO");
            if(!string.IsNullOrEmpty(ChatDTOStr))
            {
                DTO = JsonConvert.DeserializeObject<ChatHistoryItemDTO>(ChatDTOStr);
            }
            InitViews();
        }

        void InitViews()
        {
            if(DTO != null)
            {
                imgPic.Settings.JavaScriptEnabled = true;
                imgPic.LoadUrl(ServerURL.BaseURL + DTO.CONTENT);
            }
        }
    }
}
