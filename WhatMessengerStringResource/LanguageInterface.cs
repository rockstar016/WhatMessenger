using System;
using System.Collections.Generic;

namespace WhatMessengerStringResource
{
    public interface LanguageInterface
    {
        Dictionary<string, string> GetStringResourceContents();
        List<string> GetShareProfilePhoto_Resource();
        List<string> GetShareStatus_Resource();
        List<string> GetStatusTitle_Resource();
    }
}
