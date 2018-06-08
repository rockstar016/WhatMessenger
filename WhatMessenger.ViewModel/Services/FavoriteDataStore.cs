using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhatMessenger.Model;

namespace WhatMessenger.Services
{
    public class FavoriteDataStore
    {
        IList<FavorItemModel> dataProvider;
        public FavoriteDataStore()
        {
            dataProvider = new List<FavorItemModel>();
            for (int i = 0; i < 5; i++)
            {
                dataProvider.Add(new FavorItemModel { Id = Guid.NewGuid().ToString()});
                dataProvider.Add(new FavorItemModel { Id = Guid.NewGuid().ToString()});
                dataProvider.Add(new FavorItemModel { Id = Guid.NewGuid().ToString()});
                dataProvider.Add(new FavorItemModel { Id = Guid.NewGuid().ToString()});
                dataProvider.Add(new FavorItemModel { Id = Guid.NewGuid().ToString()});
                dataProvider.Add(new FavorItemModel { Id = Guid.NewGuid().ToString()});
                dataProvider.Add(new FavorItemModel { Id = Guid.NewGuid().ToString()});
                dataProvider.Add(new FavorItemModel { Id = Guid.NewGuid().ToString()});
                dataProvider.Add(new FavorItemModel { Id = Guid.NewGuid().ToString()});
                dataProvider.Add(new FavorItemModel { Id = Guid.NewGuid().ToString()});
                dataProvider.Add(new FavorItemModel { Id = Guid.NewGuid().ToString()});
            }
        }

        public async Task<FavorItemModel> GetItemAsync(string id)
        {
            return await Task.FromResult(dataProvider.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<FavorItemModel>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(dataProvider);
        }


    }
}
