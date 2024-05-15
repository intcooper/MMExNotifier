namespace MMExNotifier.DataModel
{
    internal interface IAppConfiguration
    {
        int DaysAhead { get; set; }
        string? MMExDatabasePath { get; set; }
        bool RunAtLogon { get; set; }

        void Save();
    }
}