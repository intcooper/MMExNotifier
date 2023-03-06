using Microsoft.Toolkit.Uwp.Notifications;
using MMExNotifier.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;

namespace MMExNotifier
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<ExpiringBill> ExpiringBills { get; set; }

        public MainWindow()
        {
            var t = CalculateLoanPayment(205000, 1.5726, 20, 0);

            InitializeComponent();
            DataContext = this;

            var db = new LinqToDB.Data.DataConnection(
                LinqToDB.ProviderName.SQLite,
                $"Data Source={Properties.Settings.Default.MMExDatabasePath}");

            var billDeposits = db.GetTable<BillDeposit>();
            var categories = db.GetTable<Category>();
            var payees = db.GetTable<Payee>();
            var transactions = db.GetTable<Transaction>();
            var accounts = db.GetTable<Account>();

            ExpiringBills = (from b in billDeposits
                             from s in categories.Where(s => s.CATEGID == b.CATEGID).DefaultIfEmpty()
                             from c in categories.Where(c => c.CATEGID == s.PARENTID).DefaultIfEmpty()
                             from p in payees.Where(p => p.PAYEEID == b.PAYEEID).DefaultIfEmpty()
                             where b.NEXTOCCURRENCEDATE < DateTime.Now.AddDays(7)
                             orderby b.NEXTOCCURRENCEDATE
                             select new ExpiringBill
                             {
                                 BillId = b.BDID,
                                 NextOccurrenceDate = b.NEXTOCCURRENCEDATE.Value,
                                 PayeeName = p.PAYEENAME,
                                 CategoryName = c.CATEGNAME,
                                 SubCategoryName = s.CATEGNAME,
                                 Notes = b.NOTES
                             }).ToList();

            if (!ExpiringBills.Any())
            {
                Close();
            }

            foreach (var b in ExpiringBills)
            {
                b.DaysToNextOccurrence = (int)b.NextOccurrenceDate.Subtract(DateTime.Today).TotalDays;

                new ToastContentBuilder()
                    .AddArgument("action", "viewConversation")
                    .AddArgument("conversationId", 9813 + b.BillId)
                    .AddText("Expiring recurring transaction", AdaptiveTextStyle.Header)
                    .AddText($"{b.NextOccurrenceDate.ToString("D")} - {b.PayeeName}")
                    .AddText($"{b.CategoryName}{(b.SubCategoryName != null ? ":" : string.Empty)}{b.SubCategoryName}\n{b.Notes}")
                    .SetToastScenario(ToastScenario.Reminder)
                    .Show();
            }

            var openCreditCardAccounts = accounts.Where(a => a.ACCOUNTTYPE == "Credit Card" && a.STATUS != "Closed");

            var ExpiringCreditTransactions = from ac in openCreditCardAccounts
                                             from tr in transactions.Where(t => t.ACCOUNTID == ac.ACCOUNTID
                                                                             && t.STATUS != "R" 
                                                                             && t.TRANSDATE > DateTime.Today.AddDays(-45))
                                             select tr;

        }

        private Tuple<double, double> CalculateLoanPayment(double principal, double rate, int nOfYears, int nOfPastPayments, Dictionary<int, double> earlyRepayments = null)
        {
            var loanAmount = principal;
            var periodicRate = rate / 12 / 100;
            var totPayments = 12 * nOfYears;
            var x = Math.Pow(1 + periodicRate, totPayments);
            var monthly = (loanAmount * periodicRate * x) / (x - 1);

            var balance = principal;
            var interest = principal * periodicRate;

            for (int i = 1; i <= nOfPastPayments; i++)
            {
                if (earlyRepayments.ContainsKey(i - 1))
                {
                    balance -= earlyRepayments[i - 1];
                    x = Math.Pow(1 + periodicRate, totPayments);
                    monthly = (balance * periodicRate * x) / (x - 1);
                }

                totPayments--;
                interest = balance * periodicRate;
                balance -= monthly - interest;
            }

            return new Tuple<double, double>(interest, monthly - interest);
        }
    }
}
