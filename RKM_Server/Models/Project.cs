namespace RKM_Server.Models
{
    public class Project
    {
        public int Id { get; set; }
        public ProjectStatus Type { get; set; }
        public string ProjectDescription { get; set; }
        public string Place { get; set; }
        public int OrdererId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public Orderer Orderer { get; set; }
        public ICollection<ProjectAccount> ProjectAccounts { get; set; }

    }
}

public enum ProjectStatus
{
    New,
    Draft,
    Wait,
    Scheduled,
    InPprgress,
    Completed,
    Failed
}