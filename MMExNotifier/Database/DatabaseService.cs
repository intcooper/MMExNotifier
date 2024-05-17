using LinqToDB;
using MMExNotifier.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MMExNotifier.Database
{
    internal class DatabaseService : IDatabaseService
    {
        private readonly IAppConfiguration _appConfiguration;

        public DatabaseService(IAppConfiguration appConfiguration)
        {
            _appConfiguration = appConfiguration;
        }

        public List<ExpiringBill>? ExpiringBills
        {
            get
            {
                try
                {
                    var db = new LinqToDB.Data.DataConnection(
                        ProviderName.SQLite,
                        $"Data Source={_appConfiguration.MMExDatabasePath}");

                    var billDeposits = db.GetTable<BillDeposit>();
                    var categories = db.GetTable<Category>();
                    var payees = db.GetTable<Payee>();
                    var transactions = db.GetTable<Transaction>();
                    var accounts = db.GetTable<Account>();

                    return (from b in billDeposits
                            from s in categories.Where(s => s.CATEGID == b.CATEGID).DefaultIfEmpty()
                            from c in categories.Where(c => c.CATEGID == s.PARENTID).DefaultIfEmpty()
                            from p in payees.Where(p => p.PAYEEID == b.PAYEEID).DefaultIfEmpty()
                            where b.NEXTOCCURRENCEDATE < DateTime.Now.AddDays(_appConfiguration.DaysAhead)
                            orderby b.NEXTOCCURRENCEDATE
                            select new ExpiringBill
                            {
                                BillId = b.BDID,
                                NextOccurrenceDate = b.NEXTOCCURRENCEDATE.Value,
                                PayeeName = p.PAYEENAME!,
                                CategoryName = c.CATEGNAME!,
                                SubCategoryName = s.CATEGNAME!,
                                Notes = b.NOTES!
                            }).ToList();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

    }
}
