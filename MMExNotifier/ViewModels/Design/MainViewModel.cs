using MMExNotifier.DataModel;
using MMExNotifier.MVVM;
using System;

namespace MMExNotifier.ViewModels.Design
{
    internal class MainViewModel : ViewModelBase
    {
        public RangeObservableCollection<ExpiringBill> ExpiringBills { get; set; } = new RangeObservableCollection<ExpiringBill>();

        public MainViewModel()
        {
            ExpiringBills = new RangeObservableCollection<ExpiringBill>() { 
                new ()
                {
                    BillId = 1,
                    CategoryName = "Category Name",
                    SubCategoryName = "Subcategory Name",
                    PayeeName = "Payee Name",
                    NextOccurrenceDate = new DateTime(2024, 10, 30),
                    Notes = "some notes about this entry.",
                }
            };

            OnPropertyChanged(nameof(ExpiringBills));
        }

        public override void Activate()
        {
            throw new NotImplementedException();
        }
    }
}
