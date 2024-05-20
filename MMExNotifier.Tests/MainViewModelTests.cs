
using MMExNotifier.Database;
using MMExNotifier.DataModel;
using MMExNotifier.Helpers;
using MMExNotifier.ViewModels;
using Moq;

namespace MMExNotifier.Tests
{
    public class MainViewModelTests
    {
        private Mock<IAppConfiguration> mockAppConfiguration;
        private Mock<IDatabaseService> mockDatabaseService;
        private Mock<INotificationService> mockNotificationService;

        [SetUp]
        public void Setup()
        {
            mockAppConfiguration = new Mock<IAppConfiguration>();
            mockAppConfiguration.Setup(x => x.MMExDatabasePath).Returns("C:\\mydatabase.mmb");
            mockAppConfiguration.Setup(x => x.RunAtLogon).Returns(false);
            mockAppConfiguration.Setup(x => x.DaysAhead).Returns(7);

            mockDatabaseService = new Mock<IDatabaseService>();

            mockNotificationService = new Mock<INotificationService>();
        }

        [Test]
        public void WhenDatabasePathIsNotConfigured_ShouldRaiseOpen()
        {
            mockAppConfiguration.Setup(x => x.MMExDatabasePath).Returns(string.Empty);
            mockDatabaseService.Setup(x => x.ExpiringBills).Returns(() => null);

            var mainViewModel = new MainViewModel(mockAppConfiguration.Object, mockNotificationService.Object, mockDatabaseService.Object);
            var openInvoked = false;
            mainViewModel.OnOpen += (s, e) => { openInvoked = true; };
            mainViewModel.Activate();

            Assert.That(mockDatabaseService.Invocations, Is.Empty);
            Assert.That(openInvoked, Is.True);
        }

        [Test]
        public void WhenNoExpiringBills_ShouldRaiseCloseEvent()
        {
            mockDatabaseService.Setup(x => x.ExpiringBills).Returns(() => null);

            var mainViewModel = new MainViewModel(mockAppConfiguration.Object, mockNotificationService.Object, mockDatabaseService.Object);
            var closeInvoked = false;
            mainViewModel.OnClose += (s, e) => { closeInvoked = true; };
            mainViewModel.Activate();

            Assert.That(closeInvoked, Is.True);
        }

        [Test]
        public void WhenAtLeastOneExpiringBill_ShouldShowToastNotification()
        {
            mockDatabaseService.Setup(x => x.ExpiringBills).Returns(
                new List<ExpiringBill>
                {
                    new()
                    {
                        BillId=1,
                        CategoryName="testCategory",
                        PayeeName="TestPayee",
                        NextOccurrenceDate=new DateTime(2024,6,1)
                    }
                });

            mockNotificationService.Setup(
                x => x.ShowToastNotification("viewTransactions", It.IsAny<int>(), "MMExNotifier", "One ore more recurring transaction are about to expire.", It.IsAny<Action>())
            );

            var mainViewModel = new MainViewModel(mockAppConfiguration.Object, mockNotificationService.Object, mockDatabaseService.Object);
            mainViewModel.Activate();

            mockNotificationService.Verify();
        }

        [Test]
        public void WhenNotificationActivated_ShouldRaiseOpen()
        {
            mockDatabaseService.Setup(x => x.ExpiringBills).Returns(
                new List<ExpiringBill>
                {
                    new()
                    {
                        BillId=1,
                        CategoryName="testCategory",
                        PayeeName="TestPayee",
                        NextOccurrenceDate=new DateTime(2024,6,1)
                    }
                });

            var mockToastNotification = new Mock<IToastNotification>();
            mockToastNotification.Setup(
                x => x.Show(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())
            );

            var mainViewModel = new MainViewModel(mockAppConfiguration.Object, new NotificationService(mockToastNotification.Object), mockDatabaseService.Object);
            var openInvoked = false;
            mainViewModel.OnOpen += (s, e) => { openInvoked = true; };
            mainViewModel.Activate();

            mockToastNotification.Raise(x => x.OnActivated += null, new EventArgs());

            Assert.That(openInvoked, Is.True);
        }

        [Test]
        public void OnDatabaseError_ShouldShowErrorMessage()
        {
            mockDatabaseService.Setup(x => x.ExpiringBills).Throws<InvalidOperationException>();

            mockNotificationService.Setup(
                x => x.ShowErrorNotification(It.IsAny<string>())
            );

            var mainViewModel = new MainViewModel(mockAppConfiguration.Object, mockNotificationService.Object, mockDatabaseService.Object);
            mainViewModel.Activate();

            mockNotificationService.Verify();
        }


        [Test]
        public void SavingAppSettings_ShouldReloadExpiringBills()
        {
            mockDatabaseService.Setup(x => x.ExpiringBills).Returns(
                new List<ExpiringBill>
                {
                    new()
                    {
                        BillId=1,
                        CategoryName="testCategory",
                        PayeeName="TestPayee",
                        NextOccurrenceDate=new DateTime(2024,6,1)
                    }
                });

            mockNotificationService.Setup(
                x => x.ShowErrorNotification(It.IsAny<string>())
            );

            var mainViewModel = new MainViewModel(mockAppConfiguration.Object, mockNotificationService.Object, mockDatabaseService.Object);
            mainViewModel.Activate();
            mainViewModel.SaveSettingsCommand.Execute(null);

            mockNotificationService.Verify();
            mockDatabaseService.Verify(x => x.ExpiringBills, Times.Exactly(2));
        }
    }
}