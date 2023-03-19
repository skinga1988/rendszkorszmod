﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient.Model
{
    internal class ProjectAccount_model
    {
        public int Id { get; set; }
        public ProjectAccountStatus Type { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ProjectId { get; set; }
        public Project_model Project { get; set; }
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