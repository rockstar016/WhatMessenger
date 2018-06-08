using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using API.Models.RequestModels;
using API.Models.ResponseModels;
using ModernHttpClient;
using Newtonsoft.Json;
using WhatMessenger.Model.Auth;
using WhatMessenger.Model.Constants;
using WhatMessenger.Model.RequestModels;
using WhatMessenger.ViewModel.Services;

namespace WhatMessenger.Services
{
    public class LoginDataStore : BaseDataStore
    {
        public async Task<CommonResponse> LoginServiceExecute(LoginRequestModel requestModel)
        {
            try
            {
                using (var httpClient = GetHttpClient())
                {
                    var stringContent = new List<KeyValuePair<string, string>>();
                    stringContent.Add(new KeyValuePair<string, string>("EMAIL", requestModel.EMAIL));
                    stringContent.Add(new KeyValuePair<string, string>("PASSWORD", requestModel.PASSWORD));
                    stringContent.Add(new KeyValuePair<string, string>("IMEI", requestModel.IMEI));
                    var content = new MultipartFormDataContent();
                    foreach (var keyValuePair in stringContent)
                    {
                        content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                    }
                    var response = await httpClient.PostAsync(ServerURL.LoginURL, content);
                    if (response == null) return null;
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

        public async Task<CommonResponse> PasswordChangeCommand(ChangePasswordRequest requestModel)
        {
            try
            {
                using (var httpClient = GetHttpClient())
                {
                    var stringContent = new List<KeyValuePair<string, string>>();
                    stringContent.Add(new KeyValuePair<string, string>("IMEI", requestModel.IMEI));
                    stringContent.Add(new KeyValuePair<string, string>("CODE", requestModel.CODE));
                    stringContent.Add(new KeyValuePair<string, string>("NEW_PASS", requestModel.NEW_PASS));

                    var content = new MultipartFormDataContent();
                    foreach (var keyValuePair in stringContent)
                    {
                        content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                    }
                    var response = await httpClient.PostAsync(ServerURL.DoChangePasswordURL, content);
                    if (response == null) return null;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                        return responseItem;
                    }
                }
            }
            catch (Exception exp) { }
            return null;
        }

        public async Task<CommonResponse> AskPasswordChangeCommand(AskPasswordChangeRequest requestModel)
        {
            try
            {
                using (var httpClient = GetHttpClient())
                {
                    var stringContent = new List<KeyValuePair<string, string>>();
                    stringContent.Add(new KeyValuePair<string, string>("IMEI", requestModel.IMEI));
                    stringContent.Add(new KeyValuePair<string, string>("EMAIL", requestModel.EMAIL));

                    var content = new MultipartFormDataContent();
                    foreach (var keyValuePair in stringContent)
                    {
                        content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                    }
                    var response = await httpClient.PostAsync(ServerURL.AskChangePasswordURL, content);
                    if (response == null) return null;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                        return responseItem;
                    }
                }
            }
            catch (Exception exp) { }
            return null;
        }

        public async Task<CommonResponse> LoginWithToken(GetContactRequest requestModel)
        {
            try
            {
                using (var httpClient = GetHttpClient())
                {
                    var stringContent = new List<KeyValuePair<string, string>>();
                    stringContent.Add(new KeyValuePair<string, string>("TOKEN", requestModel.TOKEN));
                    var content = new MultipartFormDataContent();
                    foreach (var keyValuePair in stringContent)
                    {
                        content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                    }
                    var response = await httpClient.PostAsync(ServerURL.TokenLoginURL, content);
                    if (response == null) return null;
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

        public async Task<CommonResponse> RegisterServiceExecute(RegisterRequest requestModel)
        {
            using (var httpClient = GetHttpClient())
            {
                var stringContent = new List<KeyValuePair<string, string>>();
                stringContent.Add(new KeyValuePair<string, string>("USERNAME", requestModel.USERNAME));
                stringContent.Add(new KeyValuePair<string, string>("USERPASSWORD", requestModel.USERPASSWORD));
                stringContent.Add(new KeyValuePair<string, string>("USERLANG", requestModel.USERLANG));
                stringContent.Add(new KeyValuePair<string, string>("EMAIL", requestModel.EMAIL));
                stringContent.Add(new KeyValuePair<string, string>("IMEI", requestModel.IMEI));
                var content = new MultipartFormDataContent();

                if(!string.IsNullOrEmpty(requestModel.FILE_NAME))
                {
                    var imageContent = new StreamContent(requestModel.FILE_STREAM);
                    content.Add(imageContent, "AVATAR", requestModel.FILE_NAME);    
                }

                foreach (var keyValuePair in stringContent)
                {
                    content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                }

                var response = await httpClient.PostAsync(ServerURL.RegisterURL, content);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                    return responseItem;
                }
            }
            return null;
        }

        //public async Task<CommonResponse> GetProfileCommandExecute(string token)
        //{
        //    using (var httpClient = GetHttpClient())
        //    {
        //        var stringContent = new List<KeyValuePair<string, string>>();
        //        var content = new MultipartFormDataContent();
        //        foreach (var keyValuePair in stringContent)
        //        {
        //            content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
        //        }
        //        var url = $"{ServerURL.}
        //        var response = await httpClient.PostAsync(ServerURL.RegisterURL, content);
        //        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //        {
        //            string retVal = await response.Content.ReadAsStringAsync();
        //            var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
        //            return responseItem;
        //        }
        //    }
        //    return null;
        //}


        public async Task<bool> TestService(LoginRequestModel requestModel)
        {
            await Task.Delay(2000);
            return await Task.FromResult(true);
        }
    }
}
