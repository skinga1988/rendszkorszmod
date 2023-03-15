namespace RKM_Server.Models
{
    public class StockAccount
    {
        public int Id { get; set; }
        public StockAccountType Type { get; set; }
        public int Pieces { get; set; }
        public DateTime AccountTime { get; set; }
        public int ProjectId { get; set; }
        public int StockItemId { get; set; }
        public Project Project { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public StockItem StockItem { get; set; }
        public ICollection<Project> Projects { get; set; }
    }
}

public enum StockAccountType
{
    Income,
    Reservation,
    PreReservation,
    Outcome

}