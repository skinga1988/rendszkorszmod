using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient.Model
{
    public class User_model
    {
        public int UserId { get; set; }
        public string RoleType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Password { get; set; }
        public ICollection<Model.Project_model> Projects { get; set; }
        public ICollection<Model.Orderer_model> Orderers { get; set; }
        public ICollection<Model.StockAccount_model> StockAccounts { get; set; }
        public ICollection<Model.ProjectAccount_model> ProjecAccounts { get; set; }
    }
}
