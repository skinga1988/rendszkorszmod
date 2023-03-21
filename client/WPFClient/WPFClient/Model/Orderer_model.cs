using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient.Model
{
    public class Orderer_model
    {
        public int Id { get; set; }
        public string OrdererName { get; set; }
        public string Description { get; set; }
        public int ProjectId { get; set; }
        public User_model User { get; set; }
        public Project_model Project { get; set; }
    }
}
