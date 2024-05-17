using System;

namespace MMExNotifier.Helpers
{
    internal interface INotificationService
    {
        public void ShowToastNotification(string actionName, int conversationId, string headerText, string message, Action onToastClickAction);
        public void ShowErrorNotification(string message);
    }
}
