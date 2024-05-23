using LinqToDB.Mapping;

namespace MMExNotifier.DataModel
{
    [Table(Name = "CATEGORY_V1")]
    internal class Category
    {
        [PrimaryKey]
        public int CATEGID { get; set; }

        [Column]
        public string? CATEGNAME { get; set; }

        [Column]
        public int ACTIVE { get; set; }

        [Column]
        public int PARENTID { get; set; }
    }
}
