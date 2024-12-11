using Microsoft.EntityFrameworkCore;

namespace donation_project.models
{
    [Owned]
    public class RefreshToken
    {
        public string Token { get; set; }

        public DateTime ExpiresOn { get; set; }

        public bool Expired => DateTime.UtcNow >= ExpiresOn;

        public DateTime CreatedOn { get; set; }

        public DateTime? RevokedOn { get; set; }

        public bool IsActive => RevokedOn == null && !Expired;

    }
}
