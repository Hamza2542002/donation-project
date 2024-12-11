using System.ComponentModel.DataAnnotations;

namespace donation_project.DTO
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
