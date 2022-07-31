using System.Collections.Generic;

namespace MyPet.BLL.DTO
{
    public class ChatDTO
    {
        public int Id { get; set; }        
        public string FirstUserId { get; set; }       
        public string SecondUserId { get; set; }
        public List<MessageDTO> Messages { get; set; }
    }
}
