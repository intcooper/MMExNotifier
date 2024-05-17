using MMExNotifier.Database;
using MMExNotifier.DataModel;
using MMExNotifier.Helpers;
using MMExNotifier.ViewModels;
using System;
using System.Threading;
using System.Windows;

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

            var appConfiguration = new AppConfiguration();

            var view = new MainWindow();
            var viewModel = new MainViewModel(appConfiguration, new NotificationService(new ToastNotification()), new DatabaseService(appConfiguration));

            view.DataContext = viewModel;
            viewModel.OnClose += (s, e) => Application.Current.Dispatcher.Invoke(() => view.Close());
            viewModel.OnOpen += (s, e) => Application.Current.Dispatcher.Invoke(() => { if (view.Visibility != Visibility.Visible) view.ShowDialog(); });
            viewModel.Activate();
        }
    }
}
