using MMExNotifier.DataModel;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Wpf.Ui.Controls;

namespace MMExNotifier.MVVM
{
    internal class ExpiringBillToInfoBadgeConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ExpiringBill expiringBill)
            {
                return null;
            }

            var infoBadge = new InfoBadge()
            {
                Icon = new SymbolIcon(SymbolRegular.Info24, filled: true),
                Severity = InfoBadgeSeverity.Informational,
                Style = Application.Current.FindResource("IconInfoBadgeStyle") as Style
            };

            if (expiringBill.IsExpired)
            {
                ((SymbolIcon)infoBadge.Icon).Symbol = SymbolRegular.Warning24;
                infoBadge.Severity = InfoBadgeSeverity.Critical;
                return infoBadge;
            }

            if (expiringBill.DaysToNextOccurrence <= 2)
            {
                ((SymbolIcon)infoBadge.Icon).Symbol = SymbolRegular.Warning24;
                infoBadge.Severity = InfoBadgeSeverity.Caution;
                return infoBadge;
            }

            return infoBadge;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
