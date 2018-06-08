using System;
using System.Net.Http;
using ModernHttpClient;
using WhatMessenger.Model.Constants;

namespace WhatMessenger.ViewModel.Services
{
    public class BaseDataStore
    {
        protected HttpClient GetHttpClient()
        {
            var httpClient = new HttpClient(new NativeMessageHandler())
            {
                BaseAddress = new Uri(ServerURL.BaseURL)
            };

            httpClient.DefaultRequestHeaders.Accept.Clear();
            return httpClient;
        }
    }
}
