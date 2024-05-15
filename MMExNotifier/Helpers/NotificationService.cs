using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Windows;
using Windows.Foundation.Collections;

namespace MMExNotifier.Helpers
{
    internal class NotificationService : INotificationService
    {
        public void ShowErrorNotification(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowToastNotification(string actionName, int conversationId, string headerText, string message, Action onToastClickAction)
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
                onToastClickAction.Invoke();
            };
        }
    }
}
