using System;

namespace MMExNotifier.Helpers
{
    internal interface IToastNotification
    {
        event EventHandler<EventArgs>? OnActivated;
        void Show(string actionName, int conversationId, string headerText, string message);
    }
}