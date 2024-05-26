using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using System.Security.Principal;
using System.IO;
using Drawing = System.Drawing;
using System.Reflection;
using WpfUi = Wpf.Ui.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms;
using Windows.UI.ApplicationSettings;

namespace MMExNotifier
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WpfUi.FluentWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Left = Properties.Settings.Default.WindowPosition.X;
            Top = Properties.Settings.Default.WindowPosition.Y;
            Width = Properties.Settings.Default.WindowSize.Width;
            Height = Properties.Settings.Default.WindowSize.Height;
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            if (settingsPanel.Visibility == Visibility.Visible)
                settingsPanel.Visibility = Visibility.Collapsed;
            else
                settingsPanel.Visibility = Visibility.Visible;
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
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select a MoneyManagerEx database file",
                Filter = "MMEx Database (*.mmb)|*.mmb",
                InitialDirectory = Path.GetDirectoryName(DbPathTextbox.Text)
            };

            if (openFileDialog.ShowDialog() == true)
            {
                DbPathTextbox.Text = openFileDialog.FileName;
            }
        }

        private void TitleBar_HelpClicked(WpfUi.TitleBar sender, RoutedEventArgs args)
        {
            var appVersion = Assembly
                .GetEntryAssembly()
                ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion ?? "unknown";

            string aboutMessage = "MMExNotifier\n" +
                $"Version {appVersion}\n\n" +
                "This software is provided free of charge and may be used, copied, and distributed without restriction.";

            System.Windows.MessageBox.Show(this, aboutMessage, "About MMExNotifier", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
