using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Amenities
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "יש להזין שם")]
        public string Name { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "המחיר לא יכול להיות שלילי")]
        public double Price { get; set; }
    }
}
