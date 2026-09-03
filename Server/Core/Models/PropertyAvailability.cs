using System;

namespace Core.Models
{
    public class PropertyAvailability
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public Properties? Property { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
