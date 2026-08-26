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
        public string Title { get; set; } 
        public string City { get; set; } 
        public string Address { get; set; }
        public double PricePerNight { get; set; }
        public int Capacity { get; set; }
        public bool IsAvailable { get; set; }
        public string Description { get; set; }
        public List<Amenities?> Amenities { get; set; }
        public List<Review?> Reviews { get; set; }
        public List<Images?> Images { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
