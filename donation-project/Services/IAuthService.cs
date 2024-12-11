using donation_project.DTO;
using donation_project.models;

namespace donation_project.Services
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(RegistrationDTO model);
        Task<AuthModel> GetTokenAsync(LoginDTO model);
        Task<AuthModel> RefreshTokenAsync(string token);
        Task<bool> RevokeRefreshTokenAsync(string token);
        Task<ForgetPasswordModel> ForgotPassword(string email);
        Task<bool> TestOtp(string otp, string email);
        Task<bool> ResetPassword(ResetPasswordDTO model);
    }
}
