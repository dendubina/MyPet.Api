namespace MyPet.BLL.DTO
{
    public class LocationDTO
    {
        public int Id { get; set; }
        public string Region { get; set; }
        public string Town { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public virtual PetDTO Pet { get; set; }
        public int PetId { get; set; }
    }
}
