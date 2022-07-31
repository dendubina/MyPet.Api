namespace MyPet.BLL.DTO
{
    public class PetDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual LocationDTO Location { get; set; }
        public int AdId { get; set; }
    }
}
