using System;
using System.IO;

namespace WhatMessenger.Droid.ChatDetailView
{
    public class ChatDetailViewUtils
    {
        public static string SetNewFileNameOfVoice(string originPath)
        {
            FileInfo fileInfo = new FileInfo(originPath);
            fileInfo.MoveTo(fileInfo.Directory.FullName + "/" + Guid.NewGuid().ToString() + ".wav");
            return fileInfo.FullName;
        }

        public ChatDetailViewUtils()
        {
        }
    }
}
