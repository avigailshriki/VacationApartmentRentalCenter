using System;

namespace Core.Resources
{
    public class PropertyAvailabilityResource
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public Core.Models.Properties? Property { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
