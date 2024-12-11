using donation_project.DTO;
using donation_project.models;
using Microsoft.AspNetCore.Identity;

namespace donation_project.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public AccountService(UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }
        public async Task<bool> DeleteAccountAsync(DeleteAccountDTO model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            
            if (user is null) return false;

            await _emailService.SendEmailAsync(user.Email, "Account Deleted", "Your Account Has Been Deleted Successfully");

            await _userManager.DeleteAsync(user);

            return true;
        }
    }
}
