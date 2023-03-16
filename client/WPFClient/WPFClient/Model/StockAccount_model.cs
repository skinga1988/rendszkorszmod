using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient.Model
{
    internal class StockAccount_model
    {
        public int Id { get; set; }
        public StockAccountType Type { get; set; }
        public int Pieces { get; set; }
        public DateTime AccountTime { get; set; }
        public int ProjectId { get; set; }
        public int StockItemId { get; set; }
        public Project_model Project { get; set; }
        public int UserId { get; set; }
        public User_model User { get; set; }
        public StockItem_model StockItem { get; set; }
        public ICollection<Project_model> Projects { get; set; }
    }
}

public enum StockAccountType
{
    Income,
    Reservation,
    PreReservation,
    Outcome

}