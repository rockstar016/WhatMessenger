using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
    public class ChatListDataStore : BaseDataStore
    {
        IList<ChatEntryDTO> EntryDTOCollection;
        Dictionary<ChatHistoryItemDTO, CancellationTokenSource> CancellableTasks;

        public ChatListDataStore()
        {
            EntryDTOCollection = new List<ChatEntryDTO>();
            CancellableTasks = new Dictionary<ChatHistoryItemDTO, CancellationTokenSource>();
        }

        public async Task<List<ChatHistoryItemDTO>> GetLoadPrivateChatHistoryCollection(LoadChatHistoryCollectionRequest model)
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
                var response = await httpClient.PostAsync(ServerURL.GetLoadPastChatHistoryURL, content);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                    if (responseItem.RESULT)
                    {
                        try
                        {
                            var resultList = JsonConvert.DeserializeObject<List<ChatHistoryItemDTO>>(responseItem.MSG);
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

        public async  Task<IEnumerable<ChatEntryDTO>> GetChatHistoriesItems(GetPrivateChatEntryRequest model)
        {
            using (var httpClient = GetHttpClient())
            {
                var stringContent = new List<KeyValuePair<string, string>>();
                stringContent.Add(new KeyValuePair<string, string>("TOKEN", model.TOKEN));
                stringContent.Add(new KeyValuePair<string, string>("MY_USER_ID", $"{model.MY_USER_ID}"));
                var content = new MultipartFormDataContent();
                foreach (var keyValuePair in stringContent)
                {
                    content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                }
                var response = await httpClient.PostAsync(ServerURL.GetMyPrivateChatEntriesURL, content);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                    if (responseItem.RESULT)
                    {
                        try
                        {
                            EntryDTOCollection.Clear();
                            EntryDTOCollection = JsonConvert.DeserializeObject<List<ChatEntryDTO>>(responseItem.MSG);
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
            }

            return EntryDTOCollection;
        }

        public async  Task<bool> UpdateItemAsync(ChatEntryDTO item)
        {
            //var _item = dataProvider.Where((ChatEntryDTO arg) => arg.EntryID == item.EntryID).FirstOrDefault();
            //_item.UnreadMessageCount = 0;
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteAllChatHistoryAsync(string token)
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

                var response = await httpClient.PostAsync(ServerURL.ClearChatHistoryURL, content);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                    return responseItem.RESULT;
                }
            }
            return false;
        }

        public async Task<bool> ClearAllSelection()
        {
          
            return await Task.FromResult(true);
        }

        public async Task<string> UploadAudioToServer(string FileName, byte[] bArray, ChatHistoryItemDTO Dto)
        {
            try
            {
                using (var httpClient = GetHttpClient())
                {
                    var stringContent = new List<KeyValuePair<string, string>>();
                    var content = new MultipartFormDataContent();
                    var imageContent = new ByteArrayContent(bArray);
                    content.Add(imageContent, "FILE", FileName);
                    foreach (var keyValuePair in stringContent)
                    {
                        content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                    }
                    CancellationTokenSource tokenSource = new CancellationTokenSource();
                    CancellableTasks.Add(Dto, tokenSource);
                    var response = await httpClient.PostAsync(ServerURL.UploadImageChatEntryURL, content, tokenSource.Token);
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

        public async Task<string> UploadImageToServer(FileData f, ChatHistoryItemDTO Dto)
        {
            try
            {
                using (var httpClient = GetHttpClient())
                {
                    var fileName = Path.GetFileName(f.FilePath);
                    var content = new MultipartFormDataContent();
                    var imageContent = new ByteArrayContent(f.DataArray);
                    content.Add(imageContent, "FILE", fileName);
                    CancellationTokenSource tokenSource = new CancellationTokenSource();
                    CancellableTasks.Add(Dto, tokenSource);
                    var response = await httpClient.PostAsync(ServerURL.UploadImageChatEntryURL, content, tokenSource.Token);
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

        public async Task<bool> StopUploading(ChatHistoryItemDTO dto)
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

        public async Task<string> UploadImageToServer(MediaFile f, ChatHistoryItemDTO Dto)
        {
            try
            {
                using (var httpClient = GetHttpClient())
                {
                    var strFileName = Path.GetFileName(f.Path);
                    var content = new MultipartFormDataContent();
                    var imageContent = new StreamContent(f.GetStream());
                    content.Add(imageContent, "FILE", strFileName);
                    try
                    {

                        CancellationTokenSource tokenSource = new CancellationTokenSource();
                        CancellableTasks.Add(Dto, tokenSource);
                        var response = await httpClient.PostAsync(ServerURL.UploadImageChatEntryURL, content, tokenSource.Token);
                        CancellableTasks.Remove(Dto);
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            string retVal = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                            var responseItem = JsonConvert.DeserializeObject<CommonResponse>(retVal);
                            if (!responseItem.RESULT) return null;
                            return responseItem.MSG;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.StackTrace);
                    }

                }
            }catch(Exception exp){}
            return null;
        }
    }
}
