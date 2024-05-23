using System;
using System.Windows;

namespace MMExNotifier.Helpers
{
    internal class NotificationService : INotificationService
    {
        private readonly IToastNotification _toastNotification;

        public NotificationService(IToastNotification toastNotification)
        {
            _toastNotification = toastNotification;    
        }

        public void ShowErrorNotification(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowToastNotification(string actionName, int conversationId, string headerText, string message, Action onToastClickAction)
        {
            _toastNotification.OnActivated += (s, e) => onToastClickAction();
            _toastNotification.Show(actionName, conversationId, headerText, message);
        }
    }
}
