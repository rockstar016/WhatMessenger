using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using WhatMessenger.Model;
using WhatMessenger.Model.BaseModel;
using API.Models.ResponseModels;
using WhatMessenger.Model.RequestModels;
using System.Net.Http;
using ModernHttpClient;
using WhatMessenger.Model.Constants;
using Newtonsoft.Json;
using API.Models.RequestModels;
using WhatMessenger.ViewModel.Services;

namespace WhatMessenger.Services
{
    public class ContactDataStore:BaseDataStore
    {
        public async Task<IList<ContactDTO>> GetMyContactAsync(string token)
        {
            IList<ContactDTO> ContactList = new List<ContactDTO>();
            using (var httpClient = GetHttpClient())
            {
                var stringContent = new List<KeyValuePair<string, string>>();
                stringContent.Add(new KeyValuePair<string, string>("TOKEN", token));
                var content = new MultipartFormDataContent();
                foreach (var keyValuePair in stringContent)
                {
                    content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                }
                var response = await httpClient.PostAsync(ServerURL.GetMyContactURL, content);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                    if (responseItem.RESULT)
                    {
                        ContactList = JsonConvert.DeserializeObject<List<ContactDTO>>(responseItem.MSG);
                    }
                }
            }
            return ContactList;
        }

        public async Task<ContactDTO> GetContactDetailAsync(GetProfileRequest model)
        {
            try
            {
                using (var httpClient = GetHttpClient())
                {
                    var stringContent = new List<KeyValuePair<string, string>>();
                    stringContent.Add(new KeyValuePair<string, string>("TOKEN", model.TOKEN));
                    stringContent.Add(new KeyValuePair<string, string>("USERID", model.USERID));
                    var content = new MultipartFormDataContent();
                    foreach (var keyValuePair in stringContent)
                    {
                        content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                    }
                    var response = await httpClient.PostAsync(ServerURL.GetContactDetailURL, content);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                        return JsonConvert.DeserializeObject<ContactDTO>(responseItem.MSG);
                    }
                }
            }
            catch(Exception exp)
            {
                
            }
            return null;
        }


        public async Task<bool> BlockAccountAsync(GetProfileRequest model)
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
                var response = await httpClient.PostAsync(ServerURL.BlockContactURL, content);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                    return responseItem.RESULT;
                }
            }
            return false;
        }

        public async Task<bool> UnblockAccountAsync(GetProfileRequest model)
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
                var response = await httpClient.PostAsync(ServerURL.UnblockContactURL, content);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                    return responseItem.RESULT;
                }
            }
            return false;
        }

        public async Task<IList<ContactDTO>> GetCandidateAsync(ContactCandidateRequest model)
        {            
            IList<ContactDTO> CandidateList = new List<ContactDTO>();
            using (var httpClient = GetHttpClient())
            {
                
                var stringContent = new List<KeyValuePair<string, string>>();
                stringContent.Add(new KeyValuePair<string, string>("TOKEN", model.TOKEN));
                stringContent.Add(new KeyValuePair<string, string>("KEYWORD", model.KEYWORD));
                var content = new MultipartFormDataContent();
                foreach (var keyValuePair in stringContent)
                {
                    content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                }
                var response = await httpClient.PostAsync(ServerURL.GetContactCandidateURL, content);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                    if(responseItem.RESULT)
                    {
                        CandidateList = JsonConvert.DeserializeObject<List<ContactDTO>>(responseItem.MSG);           
                    }
                }
            }
            return CandidateList;
        }

        public async Task<CommonResponse> AddContactAsync(ContactAddRequest model)
        {
            using (var httpClient = GetHttpClient())
            {
                var stringContent = new List<KeyValuePair<string, string>>();
                stringContent.Add(new KeyValuePair<string, string>("TOKEN", model.TOKEN));
                stringContent.Add(new KeyValuePair<string, string>("MY_ID", model.MY_ID));
                stringContent.Add(new KeyValuePair<string, string>("OTHER_ID", model.OTHER_ID));

                var content = new MultipartFormDataContent();
                foreach (var keyValuePair in stringContent)
                {
                    content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                }
                var response = await httpClient.PostAsync(ServerURL.AddContactURL, content);

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
