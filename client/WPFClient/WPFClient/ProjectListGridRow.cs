using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient
{
    internal class ProjectListGridRow
    {
        public int Id { get; set; }
        public string ProjectType { get; set; }
        public string ProjectDescription { get; set; }
        public string Place { get; set; }
        public int OrdererId { get; set; }
        public int UserId { get; set; }
    }
}
