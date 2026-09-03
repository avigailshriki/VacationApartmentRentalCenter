using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Resources
{
    public class ReviewResource
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }

        [Range(1, 5, ErrorMessage = "הדירוג חייב להיות בין 1 ל-5")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "יש להזין תגובה")]
        public string Comment { get; set; } = string.Empty;

        [Required(ErrorMessage = "יש להזין שם")]
        public string Name { get; set; } = string.Empty;

        public DateTime Date { get; set; }
    }
}
