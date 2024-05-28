using Autofac;
using MMExNotifier.Database;
using MMExNotifier.DataModel;
using MMExNotifier.Helpers;
using MMExNotifier.ViewModels;
using System.Windows;

namespace MMExNotifier
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Autofac.IContainer Container { get; set; }

        public App()
        {
            SetupContainer();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SetupContainer();

            var appConfiguration = new AppConfiguration();

            var view = new MainWindow();
            var viewModel = Container.Resolve<MainViewModel>();

            view.Hide();
            view.DataContext = viewModel;
            viewModel.OnClose += (s, e) => view.Dispatcher.Invoke(() => view.Close());
            viewModel.OnOpen += (s, e) => view.Dispatcher.Invoke(() =>
            {
                if (view.Visibility != Visibility.Visible)
                {
                    view.Visibility = Visibility.Visible;
                    view.Show();
                }
            });
            viewModel.Activate();
        }

        private void SetupContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<AppConfiguration>().As<IAppConfiguration>();
            builder.RegisterType<DatabaseService>().As<IDatabaseService>();
            builder.RegisterType<ToastNotification>().As<IToastNotification>();
            builder.RegisterType<NotificationService>().As<INotificationService>();
            builder.RegisterType<MainViewModel>().As<MainViewModel>();
            Container = builder.Build();
        }
    }
}
