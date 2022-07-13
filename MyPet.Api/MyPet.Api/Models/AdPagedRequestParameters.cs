namespace MyPet.Api.Models
{
    public class AdPagedRequestParameters : PagedRequestParameters
    {
        public string LocationRegion { get; set; } 
        public string Category { get; set; }
        public string LocationTown { get; set; }
    }
}
