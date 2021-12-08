using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.BLL.Models.EmailModels
{
    public class EmailConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool RequiresAuthentication { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string EmailConfirmationLink { get; set; }
    }
}
