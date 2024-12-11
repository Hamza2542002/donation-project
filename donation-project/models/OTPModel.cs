using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace donation_project.models
{
    public class OTPModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        [MaxLength(6)]
        public string OTP { get; set; }
        public bool isAuthenticated { get; set; }
        public DateTime ExpiresOn { get; set; }
        public bool Expired => DateTime.UtcNow > ExpiresOn;

    }
}
