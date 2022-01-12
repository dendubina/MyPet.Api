using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.BLL.DTO
{
    public class UserProfileDTO
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool TokenValidation { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string JwtToken { get; set; }
        public List<string> Roles { get; set; }

    }
}
