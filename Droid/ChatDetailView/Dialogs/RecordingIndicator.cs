using System;
using System.Threading.Tasks;
using System.Timers;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Com.Wang.Avi;
using Plugin.AudioRecorder;
using WhatMessenger.Droid.Helpers;

namespace WhatMessenger.Droid.Utils
{
    public class RecordingIndicator : Android.Support.V4.App.DialogFragment
    {
        DateTime CurrentTime;
        TextView txtName;
        Button btRecording;
        AVLoadingIndicatorView indicatorView;
        Timer timer;
        AudioRecorderService recorder;

        public event EventHandler<StringEventArgs> OnFinishRecording;

        public static RecordingIndicator GetInstance()
        {
            Bundle bundle = new Bundle();
            var Indicator = new RecordingIndicator() { Arguments = bundle };
            return Indicator;
        }

        public RecordingIndicator()
        {
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            
            var RootView = inflater.Inflate(Resource.Layout.sound_recording_dialog, container, false);
            indicatorView = RootView.FindViewById<AVLoadingIndicatorView>(Resource.Id.indicator);
            txtName = RootView.FindViewById<TextView>(Resource.Id.txtindicator);
            btRecording = RootView.FindViewById<Button>(Resource.Id.btRecord);
            btRecording.Click += BtRecording_Click;
            this.Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            this.Dialog.SetCancelable(false);
            this.Dialog.SetCanceledOnTouchOutside(false);
            this.SetStyle(DialogFragment.StyleNoTitle, 0);

            InitialScreen(false);
            recorder = new AudioRecorderService { StopRecordingOnSilence = false, StopRecordingAfterTimeout = true, TotalAudioTimeout = TimeSpan.FromMinutes(10)};
            recorder.AudioInputReceived += Recorder_AudioInputReceived;
            return RootView;
        }

        void InitialScreen(bool StartRecording)
        {
            indicatorView.Visibility = StartRecording ? ViewStates.Visible : ViewStates.Invisible;
            btRecording.Text = StartRecording ? @"Finish Recording" : @"Start Recording";    
            txtName.Text = @"Duration 00:00 / 10:00";
        }

        async void BtRecording_Click(object sender, EventArgs e)
        {
            await RecordAudio();
        }

        async Task RecordAudio()
        {
            try
            {
                if (!recorder.IsRecording)
                {
                    InitialScreen(true);
                    CurrentTime = DateTime.Now;
                    timer = new Timer();
                    timer.Interval = 1000;
                    timer.Elapsed -= Timer_Elapsed;
                    timer.Elapsed += Timer_Elapsed;
                    timer.Enabled = true;
                    await recorder.StartRecording();
                }
                else
                {
                    btRecording.Text = @"Start Recording";
                    InitialScreen(false);
                    await recorder.StopRecording();
                    this.Dismiss();
                }
            }
            catch (Exception ex)
            {
            }
        }

        void Recorder_AudioInputReceived(object sender, string e)
        {
            var file = e;
            if (OnFinishRecording != null)
                OnFinishRecording.Invoke(null, new StringEventArgs(){ FilePath = e});
        }

        void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var time = e.SignalTime.Subtract(CurrentTime);
            txtName.Text = string.Format("Duration {0:D2}:{1:D2} / 10:00", time.Minutes, time.Seconds);
        }

		public override void OnStop()
		{
            base.OnStop();
            if(timer != null)
            {
                timer.Stop();
                timer.Dispose();    
            }
		}
	}
}
