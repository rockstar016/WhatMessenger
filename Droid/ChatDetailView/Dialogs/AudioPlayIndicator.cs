using System;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Com.Wang.Avi;
using WhatMessenger.Model.Constants;

namespace WhatMessenger.Droid.ChatDetailView.Dialogs
{
    public class AudioPlayIndicator : Android.Support.V4.App.DialogFragment
    {
        public const string AUDIO_URL = "AUDIO_URL";

        ImageButton btClose, btReplay, btPause, btPlay;
        AVLoadingIndicatorView indicatorView;
        private MediaPlayer mediaPlayer;
        int playbackPosition;
        string URL;
        public static AudioPlayIndicator GetInstance(string url)
        {
            Bundle bundle = new Bundle();
            bundle.PutString(AUDIO_URL, ServerURL.BaseURL + url);
            var Indicator = new AudioPlayIndicator() { Arguments = bundle };
            return Indicator;
        }

        public AudioPlayIndicator()
        {
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            URL = this.Arguments.GetString(AUDIO_URL);
        }

        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            var RootView = inflater.Inflate(Resource.Layout.sound_play_dialog, container, false);
            indicatorView = RootView.FindViewById<AVLoadingIndicatorView>(Resource.Id.indicator);
            btClose = RootView.FindViewById<ImageButton>(Resource.Id.btClose);
            btReplay = RootView.FindViewById<ImageButton>(Resource.Id.btReplay);
            btPause = RootView.FindViewById<ImageButton>(Resource.Id.btPause );
            btPlay = RootView.FindViewById<ImageButton>(Resource.Id.btPlay);

            btClose.Click += (sender, e) => {
                try
                {
                    if (mediaPlayer != null) mediaPlayer.Stop();    
                }
                catch(Exception e3)
                {
                    
                }
                finally{
                    this.Dismiss();     
                }
            };

            btReplay.Click += (sender, e) => {
                if (mediaPlayer != null)
                {
                    indicatorView.Indicator.Start();
                    mediaPlayer.Pause();
                    mediaPlayer.SeekTo(0);
                    mediaPlayer.Start();
                    SetStatusOfImageButton(btPlay, false);
                    SetStatusOfImageButton(btPause, true);
                    SetStatusOfImageButton(btReplay, true);
                }
            };

            btPause.Click += (sender, e) =>
            {
                if (mediaPlayer != null && mediaPlayer.IsPlaying)
                {
                    indicatorView.Indicator.Stop();
                    playbackPosition = mediaPlayer.CurrentPosition;
                    mediaPlayer.Pause();
                    SetStatusOfImageButton(btPlay, true);
                    SetStatusOfImageButton(btPause, false);
                    SetStatusOfImageButton(btReplay, true);
                }
            };

            btPlay.Click += OnPlay_Click;
            this.Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            this.Dialog.SetCancelable(false);
            this.Dialog.SetCanceledOnTouchOutside(false);
            this.SetStyle(DialogFragment.StyleNoTitle, 0);

            SetStatusOfImageButton(btReplay, false);
            SetStatusOfImageButton(btPause, false);
            SetStatusOfImageButton(btPlay, true);

            indicatorView.Indicator.Stop();
            return RootView;
        }

        private void OnPlay_Click(object sender, EventArgs e)
        {
            try
            {
                if(playbackPosition != 0)
                {
                    mediaPlayer.SeekTo(playbackPosition);
                    mediaPlayer.Start();
                    playbackPosition = 0;
                }
                else
                {
                    killMediaPlayer();
                    mediaPlayer = new MediaPlayer();
                    mediaPlayer.Prepared += MediaPlayer_Prepared;
                    mediaPlayer.Completion += MediaPlayer_Completion;
                    mediaPlayer.SetDataSource(URL);
                    mediaPlayer.Prepare();
                    mediaPlayer.Start();    
                }

                SetStatusOfImageButton(btPlay, false);
                SetStatusOfImageButton(btPause, true);
                SetStatusOfImageButton(btReplay, true);

            }
            catch(Exception e12)
            {
                Toast.MakeText(this.Context, @"Unable to play this voice", ToastLength.Short).Show();
            }
        }

        public void SetStatusOfImageButton(ImageButton button, bool IsEnable)
        {
            button.SetColorFilter(new Color(ContextCompat.GetColor(this.Context, IsEnable?Resource.Color.colorPrimaryDark:Resource.Color.secondaryTextColor)), PorterDuff.Mode.SrcAtop);
            button.Enabled = IsEnable;    
        }

        void MediaPlayer_Prepared(object sender, EventArgs e)
        {
            indicatorView.Indicator.Start();
        }

        void MediaPlayer_Completion(object sender, EventArgs e)
        {
            indicatorView.Indicator.Stop();
            SetStatusOfImageButton(btPlay, false);
            SetStatusOfImageButton(btPause, false);
            SetStatusOfImageButton(btReplay, true);
        }

        public override void OnStop()
        {
            base.OnStop();
            killMediaPlayer();
        }

        private void killMediaPlayer()
        {
            if (mediaPlayer != null)
            {
                try
                {
                    mediaPlayer.Prepared -= MediaPlayer_Prepared;
                    mediaPlayer.Completion -= MediaPlayer_Completion;
                    mediaPlayer.Release();
                }
                catch (Exception e)
                {
                    
                }
            }
        }
    }
}
