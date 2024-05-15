using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MMExNotifier.DataModel
{
    internal class AppConfiguration : IAppConfiguration
    {
        public string? MMExDatabasePath { get; set; }
        public int DaysAhead { get; set; }
        public bool RunAtLogon { get; set; }

        public AppConfiguration()
        {
            MMExDatabasePath = Properties.Settings.Default.MMExDatabasePath;
            DaysAhead = Properties.Settings.Default.DaysAhead;

            using TaskService taskService = new();
            RunAtLogon = taskService.RootFolder.Tasks.Any(t => t.Name == "MMExNotifier");
        }

        public void Save()
        {
            Properties.Settings.Default.DaysAhead = DaysAhead;
            Properties.Settings.Default.MMExDatabasePath = MMExDatabasePath;

            if (RunAtLogon)
            {
                EnableSchedulerTask();
            }
            else
            {
                DisableSchedulerTask();
            }

            Properties.Settings.Default.Save();
        }

        private static void EnableSchedulerTask()
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

        private static void DisableSchedulerTask()
        {
            using TaskService taskService = new();
            taskService.RootFolder.DeleteTask("MMExNotifier", false);
        }
    }
}
