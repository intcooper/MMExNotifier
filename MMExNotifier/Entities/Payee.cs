using LinqToDB.Mapping;

namespace MMExNotifier.Entities
{
    [Table(Name="PAYEE_V1")]
    internal class Payee
    {
        [PrimaryKey]
        public int PAYEEID { get; set; }
        [Column]
        public string? PAYEENAME { get; set; }
        [Column]
        public int CATEGID { get; set; }
        [Column]
        public int SUBCATEGID { get; set; }
    }
}
