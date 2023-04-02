using Microsoft.Toolkit.Uwp.Notifications;
using System.Linq;
using System.Windows;
using Windows.Foundation.Collections;

namespace MMExNotifier
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var dbPath = MMExNotifier.Properties.Settings.Default.MMExDatabasePath;

            if (string.IsNullOrEmpty(dbPath))
            {
                var mainWindow = new MainWindow();
                return;
            }

            var daysAhead = MMExNotifier.Properties.Settings.Default.DaysAhead;
            var expiringTransactions = DbHelper.LoadRecurringTransactions(dbPath, daysAhead);

            if ((expiringTransactions != null) && (!expiringTransactions.Any()))
            {
                App.Current.Shutdown(0);
            }
            else
            {
                new ToastContentBuilder()
                    .AddArgument("action", "viewTransactions")
                    .AddArgument("conversationId", 9813)
                    .AddText($"MMExNotifier", AdaptiveTextStyle.Header)
                    .AddText($"One ore more recurring transaction are about to expire.")
                    .SetToastScenario(ToastScenario.Reminder)
                    .Show();

                // Listen to notification activation
                ToastNotificationManagerCompat.OnActivated += toastArgs =>
                {
                    ToastArguments args = ToastArguments.Parse(toastArgs.Argument);
                    ValueSet userInput = toastArgs.UserInput;
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        var mainWindow = new MainWindow();
                        mainWindow.ShowDialog();
                    });
                };
            }
        }
    }
}
