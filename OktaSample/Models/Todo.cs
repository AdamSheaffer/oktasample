using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OktaSample.Models
{
    public class Todo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
    }
}
