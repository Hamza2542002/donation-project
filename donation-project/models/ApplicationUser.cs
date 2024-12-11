using Microsoft.AspNetCore.Identity;

namespace donation_project.models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<RefreshToken>? RefreshTokens { get; set; }

        //public string Country { get; set; }
        //public string Gender { get; set; }

    }
}
