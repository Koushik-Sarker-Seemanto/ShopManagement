using System.ComponentModel.DataAnnotations;
using Models.CommonEnums;

namespace Models.AdminAuthModels
{
    public class RegisterViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Name { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public Roles Role { get; set; }
        public bool Active { get; set; }
        public string AuthorSecret { get; set; }
    }
}