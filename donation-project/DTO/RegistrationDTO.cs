using System.ComponentModel.DataAnnotations;

namespace donation_project.DTO
{
    public class RegistrationDTO
    {
        [Required]
        [MaxLength(100)]
        public string FisrtName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(100), MinLength(3)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }


    }
}
