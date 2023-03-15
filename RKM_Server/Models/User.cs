namespace RKM_Server.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string RoleType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Password { get; set; }
        public ICollection<Models.Project> Projects { get; set; }
        public ICollection<Models.Orderer> Orderers { get; set; }
        public ICollection<Models.StockAccount> StockAccounts { get; set; }
        public ICollection<Models.ProjectAccount> ProjecAccounts { get; set; }
    }
    
}
