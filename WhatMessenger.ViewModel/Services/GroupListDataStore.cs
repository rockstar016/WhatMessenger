using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using API.Models.ResponseModels;
using ModernHttpClient;
using Newtonsoft.Json;
using Plugin.FilePicker.Abstractions;
using Plugin.Media.Abstractions;
using WhatMessenger.Model;
using WhatMessenger.Model.BaseModel;
using WhatMessenger.Model.Constants;
using WhatMessenger.Model.RequestModels;
using WhatMessenger.ViewModel.Services;

namespace WhatMessenger.Services
{
    public class GroupListDataStore:BaseDataStore
    {
        List<GroupDTO> EntryDTOCollection;
        Dictionary<GroupHistoryItemDTO, CancellationTokenSource> CancellableTasks;
        public GroupListDataStore()
        {
            EntryDTOCollection = new List<GroupDTO>();
            CancellableTasks = new Dictionary<GroupHistoryItemDTO, CancellationTokenSource>();
        }

        public async Task<List<GroupHistoryItemDTO>> GetLoadGroupChatHistoryCollection(LoadChatHistoryCollectionRequest model)
        {
            using (var httpClient = GetHttpClient())
            {
                var stringContent = new List<KeyValuePair<string, string>>();
                stringContent.Add(new KeyValuePair<string, string>("TOKEN", model.TOKEN));
                stringContent.Add(new KeyValuePair<string, string>("LAST_MESSAGE_ID", $"{model.LAST_MESSAGE_ID}"));
                stringContent.Add(new KeyValuePair<string, string>("THREAD_ID", $"{model.THREAD_ID}"));

                var content = new MultipartFormDataContent();
                foreach (var keyValuePair in stringContent)
                {
                    content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                }
                var response = await httpClient.PostAsync(ServerURL.GetLoadPastGroupHistoryURL, content);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                    if (responseItem.RESULT)
                    {
                        try
                        {
                            var resultList = JsonConvert.DeserializeObject<List<GroupHistoryItemDTO>>(responseItem.MSG);
                            return resultList;
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
            }
            return null;
        }

        public async Task<CommonResponse> AddItemAsync(string token, List<int> GroupMembers, MediaFile  AvatarFile, string GroupName, string OwnerUserId, string OwnerName)
        {
            using (var httpClient = GetHttpClient())
            {
                var strMembers = JsonConvert.SerializeObject(GroupMembers);
                var stringContent = new List<KeyValuePair<string, string>>();
                stringContent.Add(new KeyValuePair<string, string>("TOKEN", token));
                stringContent.Add(new KeyValuePair<string, string>("MEMBERS", strMembers));
                stringContent.Add(new KeyValuePair<string, string>("NAME", GroupName));
                stringContent.Add(new KeyValuePair<string, string>("OwnerName", OwnerName));
                stringContent.Add(new KeyValuePair<string, string>("OwnerId", OwnerUserId));

                var content = new MultipartFormDataContent();

                if (AvatarFile != null)
                {
                    var fileName = Path.GetFileName(AvatarFile.Path);
                    var imageContent = new StreamContent(AvatarFile.GetStream());
                    content.Add(imageContent, "AVATAR", fileName);
                }

                foreach (var keyValuePair in stringContent)
                {
                    content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                }

                var response = await httpClient.PostAsync(ServerURL.AddGroupURL, content);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                    return responseItem;
                }
            }
            return null;
        }


        public async Task<bool> StopUploading(GroupHistoryItemDTO dto)
        {
            return await Task.Factory.StartNew<bool>(() =>
            {
                CancellationTokenSource workingUploadngProgress;
                if (CancellableTasks.TryGetValue(dto, out workingUploadngProgress))
                {
                    workingUploadngProgress.Cancel();
                    return true;
                }
                return false;
            });
        }

        public async Task<bool> ClearChatHistories(string token)
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
                var response = await httpClient.PostAsync(ServerURL.ClearGroupChatHistoryURL, content);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                    return responseItem.RESULT;
                }
            }
            return false;
        }

        public async Task<GroupDTO> ChangeGroupMember(string token, string GroupId, string NewGroupMembers)
        {
            try
            {
                using (var httpClient = GetHttpClient())
                {
                    var stringContent = new List<KeyValuePair<string, string>>();
                    stringContent.Add(new KeyValuePair<string, string>("TOKEN", token));
                    stringContent.Add(new KeyValuePair<string, string>("GROUP_ID", GroupId));
                    stringContent.Add(new KeyValuePair<string, string>("GROUP_VALUE", NewGroupMembers));
                    var content = new MultipartFormDataContent();
                    foreach (var keyValuePair in stringContent)
                    {
                        content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                    }

                    var response = await httpClient.PostAsync(ServerURL.UpdateGroupMember, content);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                        if (responseItem.RESULT)
                        {
                            return JsonConvert.DeserializeObject<GroupDTO>(responseItem.MSG);
                        }
                    }
                }
            }catch(Exception e){}
            return null;
        }

        public async Task<bool> ChangeGroupName(string token, string GroupId, string NewGroupName)
        {
            using (var httpClient = GetHttpClient())
            {
                var stringContent = new List<KeyValuePair<string, string>>();
                stringContent.Add(new KeyValuePair<string, string>("TOKEN", token));
                stringContent.Add(new KeyValuePair<string, string>("GROUP_ID", GroupId));
                stringContent.Add(new KeyValuePair<string, string>("GROUP_VALUE", NewGroupName));
                var content = new MultipartFormDataContent();
                foreach (var keyValuePair in stringContent)
                {
                    content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                }

                var response = await httpClient.PostAsync(ServerURL.UpdateGroupName, content);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                    return responseItem.RESULT;
                }
            }
            return false;
        }

        public async Task<bool> CloseGroup(string token, string GroupId)
        {
            using (var httpClient = GetHttpClient())
            {
                var stringContent = new List<KeyValuePair<string, string>>();
                stringContent.Add(new KeyValuePair<string, string>("TOKEN", token));
                stringContent.Add(new KeyValuePair<string, string>("USERID", GroupId));
                var content = new MultipartFormDataContent();
                foreach (var keyValuePair in stringContent)
                {
                    content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                }

                var response = await httpClient.PostAsync(ServerURL.CloseGroupURL, content);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                    return responseItem.RESULT;
                }
            }
            return false;
        }

        public async Task<IEnumerable<GroupDTO>> GetItemsAsync(string token)
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

                var response = await httpClient.PostAsync(ServerURL.GetAllGroupURL, content);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);

                    if (responseItem.RESULT)
                    {
                        EntryDTOCollection.Clear();
                        EntryDTOCollection = JsonConvert.DeserializeObject<List<GroupDTO>>(responseItem.MSG);
                    }
                }
            }
            return EntryDTOCollection;
        }


        public async Task<string> UploadAudioToServer(string FileName, byte[] bArray, GroupHistoryItemDTO Dto)
        {
            try
            {
                using (var httpClient = GetHttpClient())
                {
                    var stringContent = new List<KeyValuePair<string, string>>();
                    var content = new MultipartFormDataContent();
                    var imageContent = new ByteArrayContent(bArray);
                    content.Add(imageContent, "FILE", FileName);
                
                    CancellationTokenSource tokenSource = new CancellationTokenSource();
                    CancellableTasks.Add(Dto, tokenSource);
                    var response = await httpClient.PostAsync(ServerURL.UploadImageGroupEntryURL, content, tokenSource.Token);
                    CancellableTasks.Remove(Dto);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                        if (!responseItem.RESULT) return null;
                        return responseItem.MSG;
                    }
                }
            }catch(Exception exp){}
            return null;
        }

        public async Task<string> UploadImageToServer(MediaFile f, string token, string GroupId)
        {
            try
            {
                using (var httpClient = GetHttpClient())
                {
                    var strFileName = Path.GetFileName(f.Path);
                    var content = new MultipartFormDataContent();
                    var stringContent = new List<KeyValuePair<string, string>>();
                    stringContent.Add(new KeyValuePair<string, string>("TOKEN", token));
                    stringContent.Add(new KeyValuePair<string, string>("GROUP_ID", GroupId));
                    foreach (var keyValuePair in stringContent)
                    {
                        content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                    }

                    var imageContent = new StreamContent(f.GetStream());
                    content.Add(imageContent, "FILE", strFileName);

                    var response = await httpClient.PostAsync(ServerURL.UpdateGroupImage, content);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                        if(responseItem.RESULT)
                        {
                            return responseItem.MSG;   
                        }
                    }
                }
            }
            catch (Exception except) { }
            return null;
        }

        public async Task<string> UploadImageToServer(MediaFile f, GroupHistoryItemDTO model)
        {
            try
            {
                using (var httpClient = GetHttpClient())
                {
                    var strFileName = Path.GetFileName(f.Path);
                    var content = new MultipartFormDataContent();
                    var imageContent = new StreamContent(f.GetStream());
                    content.Add(imageContent, "FILE", strFileName);
                    CancellationTokenSource tokenSource = new CancellationTokenSource();
                    CancellableTasks.Add(model, tokenSource);
                    var response = await httpClient.PostAsync(ServerURL.UploadImageGroupEntryURL, content, tokenSource.Token);
                    CancellableTasks.Remove(model);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                        if (!responseItem.RESULT) return null;
                        return responseItem.MSG;
                    }
                }    
            }
            catch(Exception except){}
            return null;
        }

        public async Task<string> UploadImageToServer(FileData f, GroupHistoryItemDTO model)
        {
            try
            {
                using (var httpClient = GetHttpClient())
                {
                    var strFileName = Path.GetFileName(f.FilePath);
                    var content = new MultipartFormDataContent();
                    var imageContent = new ByteArrayContent(f.DataArray);
                    content.Add(imageContent, "FILE", strFileName);
                    CancellationTokenSource tokenSource = new CancellationTokenSource();
                    CancellableTasks.Add(model, tokenSource);
                    var response = await httpClient.PostAsync(ServerURL.UploadImageGroupEntryURL, content);
                    CancellableTasks.Remove(model);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                        if (!responseItem.RESULT) return null;
                        return responseItem.MSG;
                    }
                }    
            }
            catch(Exception exp){}
            return null;
        }

    }
}
