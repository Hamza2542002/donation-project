using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace donation_project.models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
        {
            
        }
        public DbSet<OTPModel> OTPs { get; set; }
        public ApplicationDbContext(DbContextOptions options):base(options)
        {
            
        }
    }
}
