﻿namespace RKM_Server.Models
{
    public class ProjectAccount
    {
        public int Id { get; set; }
        public string ProjectAccounType { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ProjectId { get; set; }
    }
}

public enum ProjectAccountStatus
{
    New,
    Draft,
    Wait,
    Scheduled,
    InPprgress,
    Completed,
    Failed
}
