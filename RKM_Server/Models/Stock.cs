namespace RKM_Server.Models
{
    public class Stock
    {
        public int Id { get; set; }
        public int RowId { get; set; }
        public int ColumnId { get; set; }
        public int BoxId { get; set; }
        public int MaxPieces { get; set; }
        public int AvailablePieces { get; set; }
        public int ReservedPieces { get; set; }
        public StockItem StockItem { get; set; }
        public ICollection<StockAccount> StockAccounts { get; set; }
    }
}