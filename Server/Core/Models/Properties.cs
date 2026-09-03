using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Properties
    {
        public int Id { get; set; }
        //[JsonIgnore]
        public int OwnerID { get; set; }
        //[ForeignKey("OwnerID")]
        //[JsonIgnore]
        public Owners? Owner { get; set; }

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

        public bool IsAvailable { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<Amenities?>? Amenities { get; set; }
        public List<Review?>? Reviews { get; set; }
        public List<Images?>? Images { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
