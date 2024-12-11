using System.ComponentModel.DataAnnotations;

namespace donation_project.DTO
{
    public class CheckOtp
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string OTP { get; set; }
    }
}
