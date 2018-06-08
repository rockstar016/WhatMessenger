using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmHelpers;
using WhatMessenger.Model;
using WhatMessenger.Services;

namespace WhatMessenger.ViewModel
{
    public class FavoriteViewModel : BaseViewModel
    {
        public FavoriteDataStore DataStore => new FavoriteDataStore();
        public ObservableCollection<FavorItemModel> Items { get; set; }
        public ICommand CommandLoadAllFavoriteListItem { get; set; } 
        public FavoriteViewModel()
        {
            Items = new ObservableCollection<FavorItemModel>();
            CommandLoadAllFavoriteListItem = new Command(async () => await ExecuteLoadAllFavorite());
        }

        async Task ExecuteLoadAllFavorite()
        {
            if (IsBusy)
                return;
            IsBusy = true;
            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
