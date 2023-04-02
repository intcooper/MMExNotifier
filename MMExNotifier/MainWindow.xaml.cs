using Microsoft.Toolkit.Uwp.Notifications;
using MMExNotifier.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32.TaskScheduler;
using LinqToDB;
using System.Security.Principal;
using Microsoft.Win32;
using System.IO;
using System.ComponentModel;
using Drawing = System.Drawing;

namespace MMExNotifier
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public List<ExpiringBill>? ExpiringBills { get; set; }

        public string MMExDatabasePath { get; set; }
        public int DaysAhead { get; set; }

        public bool RunAtLogon { get; set; }


        public MainWindow()
        {
            DaysAhead = Properties.Settings.Default.DaysAhead;
            MMExDatabasePath = Properties.Settings.Default.MMExDatabasePath;
            using TaskService taskService = new();
            RunAtLogon = taskService.RootFolder.Tasks.Any(t => t.Name == "MMExNotifier");

            InitializeComponent();
            DataContext = this;

            Left = Properties.Settings.Default.WindowPosition.X;
            Top = Properties.Settings.Default.WindowPosition.Y;
            Width = Properties.Settings.Default.WindowSize.Width;
            Height = Properties.Settings.Default.WindowSize.Height;

            LoadRecurringTransactions();
        }

        private void LoadRecurringTransactions()
        {
            try
            {
                var db = new LinqToDB.Data.DataConnection(
                    ProviderName.SQLite,
                    $"Data Source={MMExDatabasePath}");

                var billDeposits = db.GetTable<BillDeposit>();
                var categories = db.GetTable<Category>();
                var payees = db.GetTable<Payee>();
                var transactions = db.GetTable<Transaction>();
                var accounts = db.GetTable<Account>();

                ExpiringBills = (from b in billDeposits
                                 from s in categories.Where(s => s.CATEGID == b.CATEGID).DefaultIfEmpty()
                                 from c in categories.Where(c => c.CATEGID == s.PARENTID).DefaultIfEmpty()
                                 from p in payees.Where(p => p.PAYEEID == b.PAYEEID).DefaultIfEmpty()
                                 where b.NEXTOCCURRENCEDATE < DateTime.Now.AddDays(DaysAhead)
                                 orderby b.NEXTOCCURRENCEDATE
                                 select new ExpiringBill
                                 {
                                     BillId = b.BDID,
                                     NextOccurrenceDate = b.NEXTOCCURRENCEDATE!.Value,
                                     PayeeName = p.PAYEENAME!,
                                     CategoryName = c.CATEGNAME!,
                                     SubCategoryName = s.CATEGNAME!,
                                     Notes = b.NOTES!
                                 }).ToList();

                if (!ExpiringBills.Any())
                {
                    Close();
                }

                foreach (var b in ExpiringBills)
                {
                    b.DaysToNextOccurrence = (int)b.NextOccurrenceDate.Subtract(DateTime.Today).TotalDays;

                    new ToastContentBuilder()
                        .AddArgument("action", "viewConversation")
                        .AddArgument("conversationId", 9813 + b.BillId)
                        .AddText("Expiring recurring transaction", AdaptiveTextStyle.Header)
                        .AddText($"{b.NextOccurrenceDate.ToString("D")} - {b.PayeeName}")
                        .AddText($"{b.CategoryName}{(b.SubCategoryName != null ? ":" : string.Empty)}{b.SubCategoryName}\n{b.Notes}")
                        .SetToastScenario(ToastScenario.Reminder)
                        .Show();
                }

                var openCreditCardAccounts = accounts.Where(a => a.ACCOUNTTYPE == "Credit Card" && a.STATUS != "Closed");

                var ExpiringCreditTransactions = from ac in openCreditCardAccounts
                                                 from tr in transactions.Where(t => t.ACCOUNTID == ac.ACCOUNTID
                                                                                 && t.STATUS != "R"
                                                                                 && t.TRANSDATE > DateTime.Today.AddDays(-45))
                                                 select tr;
            }
            catch (Exception)
            {
                ExpiringBills = null;
                MessageBox.Show("An error has occurred while loading the recurring transactions.\nMake sure that the database version matches the supported version.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExpiringBills)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            settingsPanel.Visibility = Visibility.Visible;
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(@"MMExNotifier
Version 0.1

This software is provided free of charge and may be used, copied, and distributed without restriction.",
                "About MMExNotifier", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();

            Properties.Settings.Default.WindowPosition = new Drawing.Point((int)Left, (int)Top);
            Properties.Settings.Default.WindowSize = new Drawing.Size((int)Width, (int)Height);
            Properties.Settings.Default.Save();
        }

        private void SettingsPanelClose_Click(object sender, RoutedEventArgs e)
        {
            settingsPanel.Visibility = Visibility.Collapsed;

            Properties.Settings.Default.DaysAhead = DaysAhead;
            Properties.Settings.Default.MMExDatabasePath = MMExDatabasePath;
            Properties.Settings.Default.Save();

            LoadRecurringTransactions();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void RunOnLogon_Checked(object sender, RoutedEventArgs e)
        {
            using TaskService taskService = new();
            TaskDefinition taskDefinition = taskService.NewTask();

            // Set the task settings
            taskDefinition.RegistrationInfo.Description = "MMExNotifier";
            var userId = WindowsIdentity.GetCurrent().Name;

            // Set the trigger to run on logon
            LogonTrigger logonTrigger = new() { UserId = userId };
            taskDefinition.Triggers.Add(logonTrigger);

            // Set the action to run the executable that creates the task
            string executablePath = Environment.ProcessPath ?? "";
            taskDefinition.Actions.Add(new ExecAction(executablePath));

            // Register the task in the Windows Task Scheduler
            taskService.RootFolder.RegisterTaskDefinition("MMExNotifier", taskDefinition);
        }

        private void RunOnLogon_Unchecked(object sender, RoutedEventArgs e)
        {
            using TaskService taskService = new();
            taskService.RootFolder.DeleteTask("MMExNotifier", false);
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select a file",
                Filter = "MMEx Database (*.mmb)|*.mmb",
                InitialDirectory = Path.GetDirectoryName(MMExDatabasePath)
            };

            if (openFileDialog.ShowDialog() == true)
            {
                MMExDatabasePath = openFileDialog.FileName;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MMExDatabasePath)));
            }
        }
    }
}
