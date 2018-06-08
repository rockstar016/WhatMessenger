using System;
using System.Collections.Generic;

namespace WhatMessengerStringResource
{
    public class ChineseStringResouce : LanguageInterface
    {
        private static ChineseStringResouce Instance;

        public static ChineseStringResouce GetInstance() {
            if(Instance == null)
                Instance = new ChineseStringResouce();
            return Instance;
        }

        public Dictionary<string, string> GetStringResourceContents()
        {
            Dictionary<string, string> StringResources = new Dictionary<string, string>();
            StringResources.Add("HeaderTitleLogin", "输入您的电子邮件和密码");
            StringResources.Add("HeaderLoginDescription", "输入您的电子邮件和密码");
            StringResources.Add("EmailPlaceHolder", "电子邮件");
            StringResources.Add("PasswordPlaceHolder", "密码");
            StringResources.Add("RegisterTitle", "还没有帐号？在此注册.");
            StringResources.Add("LoginTitle", "登录");
            StringResources.Add("ForgotPasswordTitle", "忘记密码?");
            StringResources.Add("HeaderTitleRegister", "电子邮件和密码。");
            StringResources.Add("HeaderRegisterDescription", "提供您的电子邮件地址和密码。");
            StringResources.Add("Next", "下一步");
            StringResources.Add("welcome", "欢迎来到NightOwl");
            StringResources.Add("view_privacy", "点按“同意并继续”以接受隐私条款和隐私");
            StringResources.Add("agree_terms", "同意并继续");
            StringResources.Add("txtHeader", "个人资料信息");
            StringResources.Add("txtDescription", "请提供您的姓名和可选的个人资料照片");
            StringResources.Add("txtName", "于此处输入您的姓名");
            StringResources.Add("menuHome", "家");
            StringResources.Add("menuFavorite", "最爱");
            StringResources.Add("menuStatus", "状态");
            StringResources.Add("menuAccount", "帐户");
            StringResources.Add("menuSetting", "设置");
            StringResources.Add("menuChats", "聊天");
            StringResources.Add("menuCalls", "通话");
            StringResources.Add("menuGroup", "小组");
            StringResources.Add("menuContact", "联系");
            StringResources.Add("changeLang", "改变语言");
            StringResources.Add("changeWallPaper", "聊天壁纸");
            StringResources.Add("notification", "通知");
            StringResources.Add("clearChat", "清除所有聊天");
            StringResources.Add("privacy", "隐私");
            StringResources.Add("profile", "个人信息");
            StringResources.Add("deleteAccount", "删除帐户");
            StringResources.Add("changePassword", "更改密码");
            StringResources.Add("logout", "登出");
            StringResources.Add("whocansee", "谁可以看到我的个人信息");
            StringResources.Add("whostatusupdate", "谁可以看到你的状态更新");
            StringResources.Add("statusexplain", "您的隐私设置更改不会影响您已发送的状态更新");
            StringResources.Add("profile_photo", "头像");
            StringResources.Add("status", "状态");
            StringResources.Add("messaging", "消息");
            StringResources.Add("blocked_contact", "被阻止的联系人");
            StringResources.Add("list_of_contact", "您被阻止的联系人列表");
            return StringResources;
        }

        public List<string> GetShareProfilePhoto_Resource()
        {
            List<string> stringResource = new List<string>();
            stringResource.Add("大家");
            stringResource.Add("联系人");
            stringResource.Add("没人");
            return stringResource;
        }

        public List<string> GetShareStatus_Resource()
        {
            List<string> stringResource = new List<string>();
            stringResource.Add("大家");
            stringResource.Add("联系人");
            return stringResource;
        }

        public List<string> GetStatusTitle_Resource()
        {
            List<string> stringResource = new List<string>();
            stringResource.Add("可用");
            stringResource.Add("在学校");
            stringResource.Add("在电影里");
            stringResource.Add("工作中");
            stringResource.Add("电池快要耗尽了");
            stringResource.Add("不能说话，NightOwl只可用");
            stringResource.Add("在开会");
            stringResource.Add("在健身房");
            stringResource.Add("睡眠");
            stringResource.Add("仅限紧急呼叫");
            return stringResource;
        }
    }
}
