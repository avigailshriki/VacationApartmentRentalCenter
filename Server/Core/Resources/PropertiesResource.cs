using Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Core.Resources
{
    public class PropertiesResource
    {
        [Key]
        public int Id { get; set; }
        public OwnersResource? Owner { get; set; }
        public int OwnerID { get; set; }
        public int ImageId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double PricePerNight { get; set; }
        public int Capacity { get; set; }
        public bool IsAvailable { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<Images> Images { get; set; } 
        public List<AmenitiesResource> Amenities { get; set; } 
        public List<ReviewResource> Reviews { get; set; } 
       
    }
}