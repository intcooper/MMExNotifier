using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMExNotifier.Entities
{
    [Table(Name = "CHECKINGACCOUNT_V1")]
    internal class Transaction
    {
        [PrimaryKey]
        public int TRANSID { get; set; }
        [Column]
        public int ACCOUNTID { get; set; }
        [Column]
        public int? TOACCOUNTID { get; set; }
        [Column]
        public int PAYEEID { get; set; }
        [Column]
        public string TRANSCODE { get; set; }/* Withdrawal, Deposit, Transfer */
        [Column]
        public double TRANSAMOUNT { get; set; }
        [Column]
        public string STATUS { get; set; } /* None, Reconciled, Void, Follow up, Duplicate */
        [Column]
        public string TRANSACTIONNUMBER { get; set; }
        [Column]
        public string NOTES { get; set; }
        [Column]
        public int CATEGID { get; set; }
        [Column]
        public int SUBCATEGID { get; set; }
        [Column]
        public DateTime TRANSDATE { get; set; }
        [Column]
        public int FOLLOWUPID { get; set; }
        [Column]
        public double TOTRANSAMOUNT { get; set; }
    }
}
