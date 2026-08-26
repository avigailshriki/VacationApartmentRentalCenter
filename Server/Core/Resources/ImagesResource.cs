using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Resources
{
    public class ImagesResource
    {
        public int ImageId { get; set; }
        public string ImageUrl { get; set; }
        public string? AltText { get; set; }
        public int PropertyId { get; set; }
        public Properties Property { get; set; }

    }
}
