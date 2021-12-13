using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyPet.Api.Models
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Неверный email")]
        public string Email { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "Максимальная длина - 10 символов")]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [Required]
        public string PasswordConfirm { get; set; }
    }
}
