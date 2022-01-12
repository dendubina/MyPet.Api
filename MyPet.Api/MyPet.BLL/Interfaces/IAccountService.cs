using MyPet.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.BLL.Interfaces
{
    public interface IAccountService
    {
        Task<object> CreateUser(string email, string username, string password);
        Task<UserProfileDTO> SignIn(string email, string password);
        Task<bool> ConfirmEmail(string userId, string emailToken);
        Task<UserProfileDTO> CheckToken(string jwttoken);
    }
}
