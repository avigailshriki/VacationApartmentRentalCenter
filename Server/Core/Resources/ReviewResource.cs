using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Resources
{
    public class ReviewResource
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; } 
    }
}
