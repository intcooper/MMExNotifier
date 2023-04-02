using MMExNotifier.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using System.Security.Principal;
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

        public event PropertyChangedEventHandler? PropertyChanged;

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            settingsPanel.Visibility = Visibility.Visible;
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            const string aboutMessage = "MMExNotifier\n" +
                "Version 0.1\n\n" +
                "This software is provided free of charge and may be used, copied, and distributed without restriction.";

            MessageBox.Show(this, aboutMessage, "About MMExNotifier", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void LoadRecurringTransactions()
        {
            try
            {
                ExpiringBills = DbHelper.LoadRecurringTransactions(MMExDatabasePath, DaysAhead);
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
