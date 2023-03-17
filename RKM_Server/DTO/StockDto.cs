namespace RKM_Server.DTO
{
    public class StockDto
    {
        public int Id { get; set; }
        public int RowId { get; set; }
        public int ColumnId { get; set; }
        public int BoxId { get; set; }
        public int MaxPieces { get; set; }
        public int AvailablePieces { get; set; }
        public int ReservedPieces { get; set; }
        public int StockItemId { get; set; }
    }
}
