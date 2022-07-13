using System.ComponentModel.DataAnnotations;

namespace MyPet.Api.Models
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(10)]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

        [Compare("Password")]
        [Required]
        public string PasswordConfirm { get; set; }
    }
}
