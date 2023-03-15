namespace RKM_Server.Models
{
    public class Orderer
    {
        public int Id { get; set; }
        public string OrdererName { get; set; }
        public string Description { get; set; }
        public User User { get; set; }
        public Project Project { get; set; }

    }
}
