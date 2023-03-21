using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient.Model
{
    public class ProjectAccount_model
    {
        public int Id { get; set; }

        [JsonProperty("projectAccounType")]
        [JsonConverter(typeof(StringEnumConverter))]
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