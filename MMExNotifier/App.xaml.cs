using Microsoft.Toolkit.Uwp.Notifications;
using MMExNotifier.Database;
using MMExNotifier.DataModel;
using MMExNotifier.Helpers;
using MMExNotifier.ViewModels;
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

            var view = new MainWindow();
            var viewModel = new MainViewModel(new AppConfiguration(), new NotificationService());

            view.DataContext = viewModel;
            viewModel.OnClose += (s, e) => view.Close();
            viewModel.OnOpen += (s, e) => view.ShowDialog();
        }
    }
}
