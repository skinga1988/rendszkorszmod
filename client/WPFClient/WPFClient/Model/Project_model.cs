using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient.Model
{
    public class Project_model
    {
        public int Id { get; set; }
        [JsonProperty("projectType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ProjectStatus Type { get; set; }
        public string ProjectDescription { get; set; }
        public string Place { get; set; }
        public int OrdererId { get; set; }
        public int UserId { get; set; }
        public User_model User { get; set; }
        public Orderer_model Orderer { get; set; }
        public ICollection<ProjectAccount_model> ProjectAccounts { get; set; }
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