using LinqToDB;
using MMExNotifier.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MMExNotifier
{
    internal static class DbHelper
    {
        public static List<ExpiringBill>? LoadRecurringTransactions(string mmexDatabasePath, int daysAhead)
        {
            try
            {
                var db = new LinqToDB.Data.DataConnection(
                    ProviderName.SQLite,
                    $"Data Source={mmexDatabasePath}");

                var billDeposits = db.GetTable<BillDeposit>();
                var categories = db.GetTable<Category>();
                var payees = db.GetTable<Payee>();
                var transactions = db.GetTable<Transaction>();
                var accounts = db.GetTable<Account>();

                return (from b in billDeposits
                        from s in categories.Where(s => s.CATEGID == b.CATEGID).DefaultIfEmpty()
                        from c in categories.Where(c => c.CATEGID == s.PARENTID).DefaultIfEmpty()
                        from p in payees.Where(p => p.PAYEEID == b.PAYEEID).DefaultIfEmpty()
                        where b.NEXTOCCURRENCEDATE < DateTime.Now.AddDays(daysAhead)
                        orderby b.NEXTOCCURRENCEDATE
                        select new ExpiringBill
                        {
                            BillId = b.BDID,
                            NextOccurrenceDate = b.NEXTOCCURRENCEDATE!.Value,
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
