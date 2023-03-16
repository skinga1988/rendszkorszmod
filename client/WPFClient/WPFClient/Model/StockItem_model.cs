using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient.Model
{
    internal class StockItem_model
    {
        public int Id { get; set; }
        public int ItemPrice { get; set; }
        public string ItemType { get; set; }
        public ICollection<Stock_model> Stocks { get; set; }
        public ICollection<StockAccount_model> StocksAccounts { get; set; }
    }
}
