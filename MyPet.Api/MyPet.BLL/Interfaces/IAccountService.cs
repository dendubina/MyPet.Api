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
        Task<object> SignIn(string email, string password);
        Task<bool> ConfirmEmail(string userId, string emailToken);
        Task<object> CheckToken(string jwttoken);
    }
}
