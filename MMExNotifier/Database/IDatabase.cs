using MMExNotifier.DataModel;
using System.Collections.Generic;

namespace MMExNotifier.Database
{
    internal interface IDatabase
    {
        List<ExpiringBill>? ExpiringBills { get; }
    }
}