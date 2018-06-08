using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using API.Models.RequestModels;
using API.Models.ResponseModels;
using ModernHttpClient;
using Newtonsoft.Json;
using Plugin.Media.Abstractions;
using WhatMessenger.Model.Constants;
using WhatMessenger.ViewModel.Services;

namespace WhatMessenger.Services
{
    public class ProfileDataStore:BaseDataStore
    {
        public async Task<ProfileResponse> GetProfile(GetProfileRequest model)
        {
            using (var httpClient = GetHttpClient())
            {
                var stringContent = new List<KeyValuePair<string, string>>();
                stringContent.Add(new KeyValuePair<string, string>(@"TOKEN", model.TOKEN));
                stringContent.Add(new KeyValuePair<string, string>(@"USERID", model.USERID));
                var content = new MultipartFormDataContent();
                foreach (var keyValuePair in stringContent)
                {
                    content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                }
                var response = await httpClient.PostAsync(ServerURL.GetProfileURL, content);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string retVal = await response.Content.ReadAsStringAsync();
                    var responseItem = JsonConvert.DeserializeObject<ProfileResponse>(retVal);
                    return responseItem;
                }
            }
            return null;
        }

        public async Task<CommonResponse> CloseAccount(string token)
        {
            using (var httpClient = GetHttpClient())
            {
                var stringContent = new List<KeyValuePair<string, string>>();
                stringContent.Add(new KeyValuePair<string, string>("TOKEN", token));
                var content = new MultipartFormDataContent();
                foreach (var keyValuePair in stringContent)
                {
                    content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                }
                var response = await httpClient.PostAsync(ServerURL.CloseAccountURL, content);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                    return responseItem;
                }
            }
            return null;
        }

        public async Task<CommonResponse> SignOutAccount(string token)
        {
            using (var httpClient = GetHttpClient())
            {
                var stringContent = new List<KeyValuePair<string, string>>();
                stringContent.Add(new KeyValuePair<string, string>("TOKEN", token));
                var content = new MultipartFormDataContent();
                foreach (var keyValuePair in stringContent)
                {
                    content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                }
                var response = await httpClient.PostAsync(ServerURL.SignOutAccountURL, content);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                    return responseItem;
                }
            }
            return null;
        }

        public async Task<CommonResponse> UpdateUserName(GetProfileRequest model)
        {
            using (var httpClient = GetHttpClient())
            {
                var stringContent = new List<KeyValuePair<string, string>>();
                stringContent.Add(new KeyValuePair<string, string>("TOKEN", model.TOKEN));
                stringContent.Add(new KeyValuePair<string, string>("VALUE", model.USERID));
                var content = new MultipartFormDataContent();
                foreach (var keyValuePair in stringContent)
                {
                    content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                }
                var response = await httpClient.PostAsync(ServerURL.ChangeNameURL, content);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                    return responseItem;
                }
            }
            return null;
        }


        public async Task<CommonResponse> UpdateUserStatusTitle(GetProfileRequest model)
        {
            using (var httpClient = GetHttpClient())
            {
                var stringContent = new List<KeyValuePair<string, string>>();
                stringContent.Add(new KeyValuePair<string, string>("TOKEN", model.TOKEN));
                stringContent.Add(new KeyValuePair<string, string>("VALUE", model.USERID));
                var content = new MultipartFormDataContent();
                foreach (var keyValuePair in stringContent)
                {
                    content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                }
                var response = await httpClient.PostAsync(ServerURL.ChangeStatusTitleURL, content);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                    return responseItem;
                }
            }
            return null;
        }


        public async Task<CommonResponse> UpdateUserProfileShareTo(GetProfileRequest model)
        {
            try
            {
                using (var httpClient = GetHttpClient())
                {
                    var stringContent = new List<KeyValuePair<string, string>>();
                    stringContent.Add(new KeyValuePair<string, string>("TOKEN", model.TOKEN));
                    stringContent.Add(new KeyValuePair<string, string>("VALUE", model.USERID));
                    var content = new MultipartFormDataContent();
                    foreach (var keyValuePair in stringContent)
                    {
                        content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                    }
                    var response = await httpClient.PostAsync(ServerURL.ChangeProfileShareToURL, content);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                        return responseItem;
                    }
                }
            }catch(Exception exp){}
            return null;
        }

        public async Task<CommonResponse> UpdateUserStatusShareTo(GetProfileRequest model)
        {
            try
            {
                using (var httpClient = GetHttpClient())
                {
                    var stringContent = new List<KeyValuePair<string, string>>();
                    stringContent.Add(new KeyValuePair<string, string>("TOKEN", model.TOKEN));
                    stringContent.Add(new KeyValuePair<string, string>("VALUE", model.USERID));
                    var content = new MultipartFormDataContent();
                    foreach (var keyValuePair in stringContent)
                    {
                        content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                    }
                    var response = await httpClient.PostAsync(ServerURL.ChangeStatusShareToURL, content);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                        return responseItem;
                    }
                }
            }catch(Exception e){}
            return null;
        }

        public async Task<CommonResponse> UpdateUserLang(GetProfileRequest model)
        {
            try
            {
                using (var httpClient = GetHttpClient())
                {
                    var stringContent = new List<KeyValuePair<string, string>>();
                    stringContent.Add(new KeyValuePair<string, string>("TOKEN", model.TOKEN));
                    stringContent.Add(new KeyValuePair<string, string>("VALUE", model.USERID));
                    var content = new MultipartFormDataContent();
                    foreach (var keyValuePair in stringContent)
                    {
                        content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                    }
                    var response = await httpClient.PostAsync(ServerURL.ChangeLangURL, content);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                        return responseItem;
                    }
                }
            }catch(Exception exp){}
            return null;
        }

        public async Task<CommonResponse> UpdateUserPhoto(string token, MediaFile PhotoFile)
        {
            try
            {
                using (var httpClient = GetHttpClient())
                {
                    var stringContent = new List<KeyValuePair<string, string>>();
                    stringContent.Add(new KeyValuePair<string, string>("TOKEN", token));
                    var content = new MultipartFormDataContent();
                    foreach (var keyValuePair in stringContent)
                    {
                        content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                    }

                    if (!string.IsNullOrEmpty(PhotoFile.Path))
                    {
                        var fileName = Path.GetFileName(PhotoFile.Path);
                        var imageContent = new StreamContent(PhotoFile.GetStream());
                        content.Add(imageContent, "AVATAR", fileName);
                    }

                    var response = await httpClient.PostAsync(ServerURL.ChagnePhotoURL, content);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                        return responseItem;
                    }
                }
            }catch(Exception exp){}
            return null;
        }

        public async Task<CommonResponse> UpdateUserPhone(GetProfileRequest model)
        {
            try
            {
                using (var httpClient = GetHttpClient())
                {
                    var stringContent = new List<KeyValuePair<string, string>>();
                    stringContent.Add(new KeyValuePair<string, string>("TOKEN", model.TOKEN));
                    stringContent.Add(new KeyValuePair<string, string>("VALUE", model.USERID));
                    var content = new MultipartFormDataContent();
                    foreach (var keyValuePair in stringContent)
                    {
                        content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                    }
                    var response = await httpClient.PostAsync(ServerURL.ChangePhoneURL, content);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                        return responseItem;
                    }
                }
            }catch(Exception exp){}
            return null;
        }

        public async Task<CommonResponse> UpdateUserPassword(ResetPasswordRequest model)
        {
            using (var httpClient = GetHttpClient())
            {
                var stringContent = new List<KeyValuePair<string, string>>();
                stringContent.Add(new KeyValuePair<string, string>("EMAIL", model.EMAIL));
                stringContent.Add(new KeyValuePair<string, string>("IMEI", model.IMEI));
                stringContent.Add(new KeyValuePair<string, string>("OLD_PASS", model.OLD_PASS));
                stringContent.Add(new KeyValuePair<string, string>("NEW_PASS", model.NEW_PASS));

                var content = new MultipartFormDataContent();
                foreach (var keyValuePair in stringContent)
                {
                    content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                }
                var response = await httpClient.PostAsync(ServerURL.ChangePassURL, content);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                    return responseItem;
                }
            }
            return null;
        }
    }
}
