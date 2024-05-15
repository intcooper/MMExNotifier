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

        public RangeObservableCollection<ExpiringBill> ExpiringBills { get; set; } = new RangeObservableCollection<ExpiringBill>();
        public IAppConfiguration AppSettings { get; private set; }
        public RelayCommand SaveSettingsCommand;

        public MainViewModel(IAppConfiguration appSettings, INotificationService notificationService)
        {
            _notificationService = notificationService;
            AppSettings = appSettings;
            OnPropertyChanged(nameof(AppSettings));

            SaveSettingsCommand = new(() => SaveSettings());

            LoadRecurringTransactions();

            if (ExpiringBills.Count == 0)
            {
                Close();
            }

            _notificationService.ShowToastNotification("viewTransactions", 9813, "MMExNotifier", "One ore more recurring transaction are about to expire.", () => Open());
        }

        private void LoadRecurringTransactions()
        {
            try
            {
                var expiringTransactions = DbHelper.LoadRecurringTransactions(AppSettings.MMExDatabasePath, AppSettings.DaysAhead);
                ExpiringBills.AddRange(expiringTransactions);
            }
            catch (Exception)
            {
                _notificationService.ShowErrorNotification("An error has occurred while loading the recurring transactions.\nMake sure that the database version matches the supported version.");
            }
        }

        private void SaveSettings()
        {
            AppSettings.Save();

            LoadRecurringTransactions();
        }
    }
}

