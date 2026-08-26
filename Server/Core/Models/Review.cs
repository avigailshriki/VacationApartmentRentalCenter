using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        //migration אפשר למחוק ולעשות 
        public Properties Property { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        
    }
}
