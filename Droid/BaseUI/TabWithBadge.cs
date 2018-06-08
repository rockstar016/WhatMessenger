using System;
using System.Runtime.Remoting.Contexts;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Rock.Utils;
using static Android.Support.Design.Widget.TabLayout;

namespace WhatMessenger.Droid.BaseUI
{
    public class TabWithBadge
    {
        TabLayout.Tab tabView;
        Android.Content.Context context;
        string title;
        int badge;

        public TabWithBadge(Android.Content.Context context, TabLayout.Tab tab, string title, int badge)
        {
            this.tabView = tab;
            this.context = context;
            this.title = title;
            this.badge = badge;
            InitTabView();

        }


        private void InitTabView()
        {
            View rootView = LayoutInflater.From(context).Inflate(Resource.Layout.tab_with_badge, null);
            tabView.SetCustomView(rootView);
            SetBadgeCount(badge);
            SetTitleContent(title);
        }

        public TabLayout.Tab TabView => tabView;
        public void SetBadgeCount(int count)
        {
            var TabNotifyText = tabView.CustomView.FindViewById<TextView>(Resource.Id.txtTabNotification);
            if (count == Convert.ToInt16(TabNotifyText.Text))
                return;
            badge = count;
            if(badge > 0)
            {
                TabNotifyText.Visibility = ViewStates.Visible;
                AnimUtils.ShowAnimationWithAlpha(TabNotifyText, ()=>{
                    TabNotifyText.Text = $"{count}"; 
                });
                //tabView.CustomView.FindViewById<TextView>(Resource.Id.txtTabNotification).Visibility = ViewStates.Visible;

            }
            else
            {
                TabNotifyText.Text = "0";
                TabNotifyText.Visibility = ViewStates.Gone;  
            }
        }

        public void SetTitleContent(string titleContent)
        {
            this.title = titleContent;
            tabView.CustomView.FindViewById<TextView>(Resource.Id.txtTabContent).Text = title;
        }

    }
}
