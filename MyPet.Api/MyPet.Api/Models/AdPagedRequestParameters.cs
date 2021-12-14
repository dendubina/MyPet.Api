using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPet.Api.Models
{
    public class AdPagedRequestParameters : PagedRequestParameters
    {
        public string LocationRegion { get; set; } 
        public string Category { get; set; }
        public string LocationTown { get; set; }
    }
}
