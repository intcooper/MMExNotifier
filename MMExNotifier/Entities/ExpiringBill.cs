﻿using System;

namespace MMExNotifier.Entities
{
    public class ExpiringBill
    {
        public int BillId { get; set; }
        public DateTime NextOccurrenceDate { get; set; }
        public int DaysToNextOccurrence => (int)NextOccurrenceDate.Subtract(DateTime.Today).TotalDays;
        public string? PayeeName { get; set; }
        public string? CategoryName { get; set; }
        public string? SubCategoryName { get; set; }
        public string? Notes { get; set; }
    }
}
