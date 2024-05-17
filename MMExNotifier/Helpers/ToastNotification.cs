using Microsoft.Toolkit.Uwp.Notifications;
using System;
using Windows.Foundation.Collections;

namespace MMExNotifier.Helpers
{
    internal class ToastNotification : IToastNotification
    {
        public event EventHandler<EventArgs>? OnActivated;

        public void Show(string actionName, int conversationId, string headerText, string message)
        {
            new ToastContentBuilder()
               .AddArgument("action", actionName)
               .AddArgument("conversationId", conversationId)
               .AddText(headerText, AdaptiveTextStyle.Header)
               .AddText(message)
               .SetToastScenario(ToastScenario.Reminder)
               .Show();

            // Listen to notification activation
            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                ToastArguments args = ToastArguments.Parse(toastArgs.Argument);
                ValueSet userInput = toastArgs.UserInput;
                OnActivated?.Invoke(this, new EventArgs());
            };
        }
    }
}
