using System.ComponentModel.DataAnnotations;

namespace donation_project.DTO
{
    public class ResetPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password), Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }
    }
}
