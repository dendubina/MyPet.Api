using MyPet.BLL.Models.EmailModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.BLL.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendConfirmationEmail(EmailConfig config, string to, string userId, string emailToken);
    }
}
