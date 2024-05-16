using Microsoft.Toolkit.Uwp.Notifications;
using MMExNotifier.Database;
using MMExNotifier.DataModel;
using MMExNotifier.Helpers;
using MMExNotifier.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI.ApplicationSettings;

namespace MMExNotifier.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        private readonly INotificationService _notificationService;
        private readonly IDatabase _database; 

        public RangeObservableCollection<ExpiringBill> ExpiringBills { get; set; } = new RangeObservableCollection<ExpiringBill>();
        public IAppConfiguration AppSettings { get; private set; }
        public RelayCommand SaveSettingsCommand { get; private set; }

        public MainViewModel(IAppConfiguration appSettings, INotificationService notificationService, IDatabase database)
        {
            _notificationService = notificationService;
            _database = database;

            AppSettings = appSettings;
            OnPropertyChanged(nameof(AppSettings));

            SaveSettingsCommand = new(() => SaveSettings());

            if (string.IsNullOrEmpty(AppSettings.MMExDatabasePath))
                return;

            LoadExpiringBills();

            if (ExpiringBills.Count == 0)
            {
                Close();
            }

            const int ConversationId = 9813;
            _notificationService.ShowToastNotification("viewTransactions", ConversationId, "MMExNotifier", "One ore more recurring transaction are about to expire.", () => Open());
        }

        private void LoadExpiringBills()
        {
            try
            {
                ExpiringBills.Clear();
                var expiringBills = _database.ExpiringBills;
                ExpiringBills.AddRange(expiringBills);
            }
            catch (Exception)
            {
                _notificationService.ShowErrorNotification("An error has occurred while loading the recurring transactions.\nMake sure that the database version matches the supported version.");
            }
        }

        private void SaveSettings()
        {
            AppSettings.Save();

            LoadExpiringBills();
        }
    }
}

