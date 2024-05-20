using System;

namespace MMExNotifier.DataModel
{
    public class ExpiringBill
    {
        public int BillId { get; set; }
        public DateTime NextOccurrenceDate { get; set; }
        public int DaysToNextOccurrence => (int)NextOccurrenceDate.Subtract(DateTime.Today).TotalDays;
        public bool IsExpired => DaysToNextOccurrence < 0;
        public string? PayeeName { get; set; }
        public string? CategoryName { get; set; }
        public string? SubCategoryName { get; set; }
        public string? Notes { get; set; }
        public int Amount { get; set; }
        public bool IsDeposit { get; set; }
    }
}
