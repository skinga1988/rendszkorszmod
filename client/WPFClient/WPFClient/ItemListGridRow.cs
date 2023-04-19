using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient
{
    internal class ItemListGridRow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RowId { get; set; }
        public int ColumnId { get; set; }
        public int BoxId { get; set; }
    }
}
