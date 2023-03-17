namespace RKM_Server.DTO
{
    public class StockAccountDto
    {
        public int Id { get; set; }
        public string StockAccountType { get; set; }
        public int Pieces { get; set; }
        public DateTime AccountTime { get; set; }
        public int ProjectId { get; set; }
        public int StockItemId { get; set; }
        public int UserId { get; set; }
    }
}
