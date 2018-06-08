using System;
namespace WhatMessenger.Model.Constants
{
    public class ServerURL
    {
        public const string BaseURL = @"http://192.168.1.117/";
        //public const string BaseURL = @"http://103.75.191.144/";
        public const string LoginURL = @"api/auth/login";
        public const string TokenLoginURL = @"api/auth/tokenlogin";
        public const string RegisterURL = @"api/auth/register";
        public const string GetProfileURL = @"api/auth/profile";
        public const string CloseAccountURL = @"api/auth/close";
        public const string SignOutAccountURL = @"api/auth/signout";
        public const string ChangePhoneURL = @"api/auth/change_phone";
        public const string ChangeNameURL = @"api/auth/change_name";
        public const string ChagnePhotoURL = @"api/auth/change_photo";
        public const string ChangePassURL = @"api/auth/change_pass";
        public const string ChangeLangURL = @"api/auth/change_lang";
        public const string ChangeProfileShareToURL = @"api/auth/change_profile_shareto";
        public const string ChangeStatusShareToURL = @"api/auth/change_status_shareto";
        public const string ChangeStatusTitleURL = @"api/auth/change_status_title";
        public const string AskChangePasswordURL = @"api/auth/ask_changepass";
        public const string DoChangePasswordURL = @"api/auth/forgot_pass";

        public const string GetContactCandidateURL = @"api/contact/candidate";
        public const string AddContactURL = @"api/contact/add";
        public const string GetMyContactURL = @"api/contact/my";
        public const string GetContactDetailURL = @"api/contact/detail";
        public const string BlockContactURL = @"api/contact/block";
        public const string UnblockContactURL = @"api/contact/unblock";
        public const string AddGroupURL = @"api/group/add";
        public const string GetAllGroupURL = @"api/group/getall";
        public const string UpdateGroupName = @"api/group/update_name";
        public const string UpdateGroupMember = @"api/group/update_member";
        public const string UpdateGroupImage = @"api/group/update_avatar";
        public const string CloseGroupURL = @"api/group/close_group";
        public const string UploadImageGroupEntryURL = @"api/group/ent_img";
        public const string ClearGroupChatHistoryURL = @"api/group/clear_all";
        public const string GetLoadPastGroupHistoryURL = @"api/group/loadpast";

        public const string GetMyPrivateChatEntriesURL = @"api/chat/pentries";
        public const string GetLoadPastChatHistoryURL = @"api/chat/loadpast";
        public const string UploadImageChatEntryURL = @"api/chat/pent_img";
        public const string ClearChatHistoryURL = @"api/chat/clear_all";
        public const string ChatHistoryURL = BaseURL + @"api/profile/GetMessage";
    }
}
