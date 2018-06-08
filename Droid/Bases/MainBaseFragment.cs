
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using WhatMessenger.Droid.Utils;

namespace WhatMessenger.Droid.Bases
{
    public class MainBaseFragment : Android.Support.V4.App.Fragment
    {
        LoadingIndicator loadingIndicator;
        public MainActivity ParentActivity { get; set; }
        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            this.ParentActivity = context as MainActivity;
        }

        public override void OnDetach()
        {
            base.OnDetach();
            this.ParentActivity = null;
        }

        protected void ShowHideMenuSearchItem(bool value) { ParentActivity.ShowHideSearchIcon(value);}

        protected void InitSwipeRefreshLayout(SwipeRefreshLayout swiper)
        {
            swiper.SetColorSchemeResources(Resource.Color.colorPrimary, Resource.Color.colorAccent, Resource.Color.colorPrimaryDark);
        }

        protected void InitRecyclerScrollListener(RecyclerView recyclerView, Action<bool> WhenDownAction)
        {
            recyclerView.ScrollChange += (sender, e) =>
            {
                int visibleItemCount = recyclerView.GetLayoutManager().ChildCount;
                int totalItemCount = recyclerView.GetLayoutManager().ItemCount;
                int pastVisibleItems = ((LinearLayoutManager)recyclerView.GetLayoutManager()).FindFirstCompletelyVisibleItemPosition();
                if (pastVisibleItems + visibleItemCount > totalItemCount)
                {
                    //when reach to bottom
                    WhenDownAction.Invoke(true);
                }
                else
                {
                    //when reach out from bottom
                    WhenDownAction.Invoke(false);
                }
            };
        }

        protected void ShowLoadingDialog(string title)
        {
            loadingIndicator = LoadingIndicator.GetInstance(title);
            loadingIndicator.Show(ChildFragmentManager, "dialog");
        }

        protected void HideLoadingDialog()
        {
            if (loadingIndicator != null)
            {
                loadingIndicator.Dismiss();
            }
            loadingIndicator = null;
        }

    }
}
