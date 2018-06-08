using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhatMessenger.Model;

namespace WhatMessenger.Services
{
    public class CallListDataStore: IDataStore<CallListItem>
    {
        IList<CallListItem> dataProvider;

        public CallListDataStore()
        {
            dataProvider = new List<CallListItem>();
        }

        public async Task<bool> AddItemAsync(CallListItem item)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<CallListItem> GetItemAsync(string id)
        {
            return await Task.FromResult(dataProvider.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<CallListItem>> GetItemsAsync(bool forceRefresh = false)
        {
            //if forceRefresh == true => get result from api
            // else just return result
            return await Task.FromResult(dataProvider);
        }

        public async Task<bool> UpdateItemAsync(CallListItem item)
        {
            return await Task.FromResult(true);
        }

  
    }
}
