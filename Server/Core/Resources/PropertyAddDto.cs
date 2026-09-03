using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Resources
{
    public class PropertyAddDto
    {
        [Required(ErrorMessage = "יש להזין כותרת")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "יש להזין עיר")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "יש להזין כתובת")]
        public string Address { get; set; } = string.Empty;

        [Range(1, double.MaxValue, ErrorMessage = "מחיר ללילה חייב להיות גדול מ-0")]
        public double PricePerNight { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "התפוסה חייבת להיות לפחות 1")]
        public int Capacity { get; set; }

        public string Description { get; set; } = string.Empty;

    }
}
