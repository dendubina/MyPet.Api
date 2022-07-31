using MyPet.BLL.Models.EmailModels;
using System.Threading.Tasks;

namespace MyPet.BLL.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendConfirmationEmail(EmailConfig config, string to, string userId, string emailToken);
    }
}
