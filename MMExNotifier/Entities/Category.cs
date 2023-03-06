using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMExNotifier.Entities
{
    [Table(Name="CATEGORY_V1")]
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
