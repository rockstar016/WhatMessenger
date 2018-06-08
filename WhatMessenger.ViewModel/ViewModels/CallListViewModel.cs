using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmHelpers;
using WhatMessenger.Model;
using WhatMessenger.Services;

namespace WhatMessenger.ViewModel
{
    public class CallListViewModel: BaseViewModel
    {
        public IDataStore<CallListItem> DataStore => ServiceLocator.Instance.Get<IDataStore<CallListItem>>() ?? new CallListDataStore();
        public ICommand LoadAllCallListItem { get; set; }
        public ObservableRangeCollection<CallListItem> Items { get; set; }

        public CallListViewModel()
        {
            Title = "Call History";
            Items = new ObservableRangeCollection<CallListItem>();
            LoadAllCallListItem = new Command(async () => await ExecuteLoadHistory());

        }

        async Task ExecuteLoadHistory()
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

        async Task ExcuteUpdateItem(CallListItem item)
        {
            await DataStore.UpdateItemAsync(item);
            await ExecuteLoadHistory();
        }

        async Task ExcuteClearAllSelection()
        {
            await (DataStore as ChatListDataStore).ClearAllSelection();
            await ExecuteLoadHistory();
        }
    }
}
