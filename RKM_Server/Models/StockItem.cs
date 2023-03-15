namespace RKM_Server.Models
{
    public class StockItem
    {
        public int Id { get; set; }
        public int ItemPrice { get; set; }
        public string ItemType { get; set; }
        public ICollection<Stock> Stocks { get; set; }
        public ICollection<StockAccount> StocksAccounts { get; set; }
    }
}
