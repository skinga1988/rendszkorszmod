﻿namespace RKM_Server.Models
{
    public class Orderer
    {
        public int Id { get; set; }
        public string OrdererName { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }

    }
}
