using LinqToDB.Mapping;
using System;

namespace MMExNotifier.DataModel
{
    [Table(Name = "ACCOUNTLIST_V1")]
    internal class Account
    {
        [PrimaryKey]
        public int ACCOUNTID { get; set; }

        [Column]
        public string? ACCOUNTNAME { get; set; }

        [Column]
        public string? ACCOUNTTYPE { get; set; }

        [Column]
        public string? ACCOUNTNUM { get; set; }

        [Column]
        public string? STATUS { get; set; }

        [Column]
        public string? NOTES { get; set; }

        [Column]
        public string? HELDAT { get; set; }

        [Column]
        public string? WEBSITE { get; set; }

        [Column]
        public string? CONTACTINFO { get; set; }

        [Column]
        public string? ACCESSINFO { get; set; }

        [Column]
        public double INITIALBAL { get; set; }

        [Column]
        public string? FAVORITEACCT { get; set; }

        [Column]
        public int CURRENCYID { get; set; }

        [Column]
        public int STATEMENTLOCKED { get; set; }

        [Column]
        public DateTime? STATEMENTDATE { get; set; }

        [Column]

        [Column]
        public int MINIMUMBALANCE { get; set; }

        [Column]
        public double CREDITLIMIT { get; set; }

        [Column]
        public int INTERESTRATE { get; set; }

        [Column]
        public DateTime? PAYMENTDUEDATE { get; set; }

        [Column]
        public int MINIMUMPAYMENT { get; set; }
    }
}
