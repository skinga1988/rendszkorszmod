using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient.Model
{
    internal class Stock_model
    {
        public int Id { get; set; }
        public int RowId { get; set; }
        public int ColumnId { get; set; }
        public int BoxId { get; set; }
        public int MaxPieces { get; set; }
        public int AvailablePieces { get; set; }
        public int ReservedPieces { get; set; }
        public int StockItemId { get; set; }
        public StockItem_model StockItem { get; set; }
        public ICollection<StockAccount_model> StockAccounts { get; set; }
    }
}
