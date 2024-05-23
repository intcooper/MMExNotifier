using LinqToDB.Mapping;
using System;

namespace MMExNotifier.DataModel
{
    [Table(Name = "BILLSDEPOSITS_V1")]
    internal class BillDeposit
    {
        [PrimaryKey]
        public int BDID { get; set; }

        [Column]
        public int ACCOUNTID { get; set; }

        [Column]
        public int TOACCOUNTID { get; set; }

        [Column]
        public int PAYEEID { get; set; }

        [Column]
        public string TRANSCODE { get; set; } = "Withdrawal"; /* Withdrawal, Deposit, Transfer */

        [Column]
        public double TRANSAMOUNT { get; set; }

        [Column]
        public string? STATUS { get; set; } /* None, Reconciled, Void, Follow up, Duplicate */

        [Column]
        public string? TRANSACTIONNUMBER { get; set; }

        [Column]
        public string? NOTES { get; set; }

        [Column]
        public int CATEGID { get; set; }

        [Column]
        public int SUBCATEGID { get; set; }

        [Column]
        public string? TRANSDATE { get; set; }

        [Column]
        public int FOLLOWUPID { get; set; }

        [Column]
        public int TOTRANSAMOUNT { get; set; }

        [Column]
        public int REPEATS { get; set; }

        [Column]
        public DateTime NEXTOCCURRENCEDATE { get; set; }

        [Column]
        public int NUMOCCURRENCES { get; set; }
    }
}
