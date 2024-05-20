using MMExNotifier.DataModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MMExNotifier.MVVM
{
    internal class DaysToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value is not ExpiringBill)
                return string.Empty;

            var expiringBill = (ExpiringBill)value;

            if (expiringBill.DaysToNextOccurrence < -5)
                return $"expired on {expiringBill.NextOccurrenceDate:d} ({-expiringBill.DaysToNextOccurrence} days ago)!";

            if (expiringBill.DaysToNextOccurrence < -1)
                return $"expired {-expiringBill.DaysToNextOccurrence} days ago!";

            if (expiringBill.DaysToNextOccurrence == -1)
                return "expired yesterday!";

            if (expiringBill.DaysToNextOccurrence == 0)
                return "expires today.";

            if (expiringBill.DaysToNextOccurrence == 1)
                return "expires tomorrow.";

            if (expiringBill.DaysToNextOccurrence > 1)
                return $"will expire on {expiringBill.NextOccurrenceDate:d} (in {expiringBill.DaysToNextOccurrence} days).";

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
