using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Images
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string? AltText { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Today;
        public int PropertyId { get; set; }
        public virtual  Properties Property { get; set; }
    }
}
