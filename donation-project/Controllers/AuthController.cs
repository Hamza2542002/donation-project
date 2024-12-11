using donation_project.DTO;
using donation_project.Services;
using Microsoft.AspNetCore.Mvc;

namespace donation_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService, IEmailService emailService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly IEmailService _emailService = emailService;

        [HttpGet]
        public async Task<IActionResult> SaySomeThing()
        {
            return Ok("Hello To Our Application");
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegistrationDTO model)
        {
            if(!ModelState.IsValid) 
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(model);

            if(!result.IsAuthenticated) return BadRequest(result.Message);

            if (!string.IsNullOrEmpty(result.RefreshToken))
                SetRefreshTokenToCookies(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.GetTokenAsync(model);

            if(!result.IsAuthenticated) 
                return BadRequest(result.Message);

            if (!string.IsNullOrEmpty(result.RefreshToken))
                SetRefreshTokenToCookies(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(string token)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RefreshTokenAsync(token);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            if(!string.IsNullOrEmpty(result.RefreshToken))
                SetRefreshTokenToCookies(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken(string token)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RevokeRefreshTokenAsync(token);

            if (!result)
                return BadRequest("Some Thing Went Wrong");

            return Ok();
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.ForgotPassword(model.Email!);

            if(!result.SentSuccessfully)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("check-otp")]
        public async Task<IActionResult> TestOTP(CheckOtp model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.TestOtp(model.OTP, model.Email);

            if (!result)
                return BadRequest("OTP Expired");

            return Ok("OTP Verified Successfully");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ResetPassword(model);

            return result ? Ok("Password Updated Successfully") : BadRequest("Some thing went wrong");
        }

        private void SetRefreshTokenToCookies(string? refreshToken , DateTime? expires)
        {
            var cookiesOption = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires
            };

            Response.Cookies.Append("refresh-token", refreshToken,cookiesOption);
        }
    }
}
