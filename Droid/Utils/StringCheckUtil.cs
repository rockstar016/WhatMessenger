using System;
using Android.Content;
using Android.Text;
using Android.Widget;
using WhatMessenger.Droid;

namespace Rock.Utils
{
    public class StringCheckUtil
    {
		public static bool isEmpty(EditText textController)
		{
			bool returnValue = string.IsNullOrEmpty(textController.Text);
			if (returnValue)
				textController.SetError(@"Required Field", null);
			return returnValue;
		}

		public static bool isLength(EditText textController, int limitation)
		{
			bool returnValue = textController.Text.Length < limitation;
			if (returnValue)
			{
				string dispMessage = string.Format("At least {0} characters", limitation);
				textController.SetError(dispMessage, null);
			}
			return returnValue;
		}

		public static bool isEmailAddress(EditText textController)
		{

			bool retValue = Android.Util.Patterns.EmailAddress.Matcher(textController.Text).Matches();
			if (!retValue)
			{
				string dispMessage = @"Invalid Email Address";
				textController.SetError(dispMessage, null);
			}
			return retValue;
		}

		public static bool CompareText(EditText textController1, EditText textController2)
		{
			bool retValue = string.Equals(textController1.Text, textController2.Text);
			if (!retValue)
			{
				string dispMessage = @"Password doesn't match";
				textController2.SetError(dispMessage, null);
			}
			return retValue;
		}

        public static string FromHtmlToString(string htmlText)
        {
            if(Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
            {
                return Html.FromHtml(htmlText, Html.FromHtmlModeLegacy).ToString();
            }
            else
            {
                return Html.FromHtml(htmlText).ToString();
            }
        }

    }
}
