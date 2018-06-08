using System;
using System.Collections.Generic;

namespace WhatMessengerStringResource
{
    public class EnglishStringResource : LanguageInterface
    {
        private static EnglishStringResource Instance;

        public static EnglishStringResource GetInstance()
        {
            if (Instance == null)
                Instance = new EnglishStringResource();
            return Instance;
        }

        public Dictionary<string, string> GetStringResourceContents()
        {
            Dictionary<string, string> StringResources = new Dictionary<string, string>();
            StringResources.Add("HeaderTitleLogin", "Type your email and password.");
            StringResources.Add("HeaderLoginDescription", "Type your email and password.");
            StringResources.Add("EmailPlaceHolder", "Email Address");
            StringResources.Add("PasswordPlaceHolder", "Password");
            StringResources.Add("RegisterTitle", "Don't have account yet? Sign up here");
            StringResources.Add("LoginTitle", "Log in");
            StringResources.Add("ForgotPasswordTitle", "Forgot Password?");
            StringResources.Add("HeaderTitleRegister", "Email and Password.");
            StringResources.Add("HeaderRegisterDescription", "Provide your email address and password.");
            StringResources.Add("Next", "Next");
            StringResources.Add("welcome", "Welcome to NightOwl");
            StringResources.Add("view_privacy", "Tap \"Agree and continue\" to accept Privacy Terms and Privacy");
            StringResources.Add("agree_terms", "Agree and Continue");
            StringResources.Add("txtHeader", "Profile info");
            StringResources.Add("txtDescription", "Please provide your name and an optional profile photo");
            StringResources.Add("txtName", "Type your name here");
            StringResources.Add("menuHome", "Home");
            StringResources.Add("menuFavorite", "Favorites");
            StringResources.Add("menuStatus", "Status");
            StringResources.Add("menuAccount", "Account");
            StringResources.Add("menuSetting", "Setting");
            StringResources.Add("menuChats", "Chats");
            StringResources.Add("menuCalls", "Calls");
            StringResources.Add("menuGroup", "Group");
            StringResources.Add("menuContact", "Contact");
            StringResources.Add("changeLang", "Change Language");
            StringResources.Add("changeWallPaper", "Chat Wallpaper");
            StringResources.Add("notification", "Notification");
            StringResources.Add("clearChat", "Clear All Chats");
            StringResources.Add("privacy", "Privacy");
            StringResources.Add("profile", "Profile");
            StringResources.Add("deleteAccount", "Delete Account");
            StringResources.Add("changePassword", "Change Password");
            StringResources.Add("logout", "Log out");
            StringResources.Add("whocansee", "Who can see my personal info");
            StringResources.Add("whostatusupdate", "Who can see your status updates");
            StringResources.Add("statusexplain", "Changes to your privacy settings won't affect status updates that you've sent already.");
            StringResources.Add("profile_photo", "Profile Photo");
            StringResources.Add("status", "Status");
            StringResources.Add("messaging", "Messaging");
            StringResources.Add("blocked_contact", "Blocked Contacts");
            StringResources.Add("list_of_contact", "List of contacts that you have been blocked");

            return StringResources;
        }

        public List<string> GetShareProfilePhoto_Resource()
        {
            List<string> stringResource = new List<string>();
            stringResource.Add("Every One");
            stringResource.Add("My Contacts");
            stringResource.Add("No body");
            return stringResource;
        }

        public List<string> GetShareStatus_Resource()
        {
            List<string> stringResource = new List<string>();
            stringResource.Add("Every One");
            stringResource.Add("My Contacts");
            return stringResource;
        }

        public List<string> GetStatusTitle_Resource()
        {
            List<string> stringResource = new List<string>();
            stringResource.Add("Available");
            stringResource.Add("At School");
            stringResource.Add("At the movies");
            stringResource.Add("At work");
            stringResource.Add("Battery about to die");
            stringResource.Add("Can't talk, NightOwl only");
            stringResource.Add("In a meeting");
            stringResource.Add("At the gym");
            stringResource.Add("Sleeping");
            stringResource.Add("Urgent calls only");
            return stringResource;
        }
    }
}
