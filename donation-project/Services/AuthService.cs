using donation_project.DTO;
using donation_project.Helpers;
using donation_project.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace donation_project.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IEmailService _emailService;
        private readonly JWT _jWT;

        public AuthService(UserManager<ApplicationUser> userManager , RoleManager<IdentityRole> roleManager , IOptions<JWT> jwtOptions , IEmailService emailService, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _emailService = emailService;
            _jWT = jwtOptions.Value;
        }

        public async Task<AuthModel> RegisterAsync(RegistrationDTO model)
        {

            if (await _userManager.FindByEmailAsync(model.Email) != null) 
                return new AuthModel() { Message = "Email Already Exist" };

            if (await _userManager.FindByNameAsync(model.UserName) != null)
                return new AuthModel() { Message = "username Already Exist" };

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FisrtName,
                LastName = model.LastName
            };


            var result = await _userManager.CreateAsync(user, model.Password);


            if (!result.Succeeded)
            {
                var errors = new StringBuilder("");
                foreach (var error in result.Errors)
                {
                    errors.AppendLine(error.Description);
                }
                return new AuthModel { Message =  errors.ToString() };
            }

            var res = await _userManager.AddToRoleAsync(user, "user");

            if (!res.Succeeded)
                return new AuthModel { Message = "Can't Add User To role " };

            var token = await CreateJWTToken(user);

            var refreshToken = CreateRefreshToken();

            var authModel = new AuthModel
            {
                Email = model.Email,
                UserName = model.UserName,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                IsAuthenticated = true,
                ExpiresOn = token.ValidTo,
                Roles =  (List<string>) _userManager.GetRolesAsync(user).Result,
                RefreshTokenExpiration = refreshToken.ExpiresOn,
                RefreshToken = refreshToken.Token
            };

            return authModel;
        }

        public async Task<AuthModel> GetTokenAsync(LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password)) 
                return new AuthModel { Message = "Invalid Login Attempt" };

            var token = await CreateJWTToken(user);

            var authModel = new AuthModel
            {
                Email = user.Email ?? "",
                UserName = user.UserName ?? "",
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                IsAuthenticated = true,
                ExpiresOn = token.ValidTo,
                Roles = (List<string>)_userManager.GetRolesAsync(user).Result
            };

            if(user.RefreshTokens.Any(r => r.IsActive))
            {
                var refreshToken = user.RefreshTokens.FirstOrDefault(r => r.IsActive);
                authModel.RefreshToken = refreshToken?.Token;
                authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;
            }
            else
            {
                var newResfreshToken = CreateRefreshToken();
                authModel.RefreshToken = newResfreshToken.Token;
                authModel.RefreshTokenExpiration = newResfreshToken.ExpiresOn;

                user.RefreshTokens.Add(newResfreshToken);
                await _userManager.UpdateAsync(user);
            }

            return authModel;

        }

        public async Task<AuthModel> RefreshTokenAsync(string token)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(r => r.Token == token));

            if (user is null || !user.RefreshTokens.Any(r => r.Token == token && r.IsActive))
                return new AuthModel { Message = "Invalid Refresh Token" };

            var refreshToken = user.RefreshTokens.Single(r => r.Token == token);

            refreshToken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = CreateRefreshToken();

            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var newToken = await CreateJWTToken(user);

            var authModel = new AuthModel();

            authModel.Email = user.Email;
            authModel.UserName = user.UserName;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(newToken);
            authModel.IsAuthenticated = true;
            authModel.ExpiresOn = newToken.ValidTo;
            var roles = await _userManager.GetRolesAsync(user);
            authModel.Roles = roles.ToList();

            return authModel;
        }

        public async Task<bool> RevokeRefreshTokenAsync(string token)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(r => r.Token == token && r.IsActive));

            if (user is null)
                return false;

            var refreshToken = user.RefreshTokens?.Single(r => r.Token == token);

            refreshToken.RevokedOn = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            return true;
        }

        public async Task<ForgetPasswordModel> ForgotPassword(string? email)
        {
            var user = await _userManager.FindByEmailAsync(email!);

            if (user is null)
                return new ForgetPasswordModel {Message = "Invalid Email address" };

            var otpModel = CreateOTP();

            otpModel.UserId = user.Id;
            await _dbContext.AddAsync(otpModel);
            await _dbContext.SaveChangesAsync();

            try
            {
                await _emailService.SendEmailAsync(email, "Email Verification", otpModel.OTP);
            }
            catch
            {
                return new ForgetPasswordModel { Message = "Coudln't Send Email, Please Try again" };
            }
            return new ForgetPasswordModel
            {
                Message = "OTP sent to your Email",
                OTPExpiration = otpModel.ExpiresOn,
                SentSuccessfully = true
            };
        }

        public async Task<bool> TestOtp(string otp, string email)
        {
            if (otp is null || email is null || otp.Length == 0 || email.Length == 0)
                return false;

            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return false;

            var Otp = await _dbContext.OTPs.FirstOrDefaultAsync(o => o.OTP == otp && user.Id == o.UserId);

            if (Otp is null || Otp.Expired)
                return false;

            Otp.isAuthenticated = true;
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task<bool> ResetPassword(ResetPasswordDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null) return false;

            var otp = await _dbContext.OTPs.OrderBy(o => o.Id).LastOrDefaultAsync(o => user.Id == o.UserId);

            if (otp is null || !otp.isAuthenticated)
                return false;

            try
            {
                await _userManager.RemovePasswordAsync(user);
                var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
               
            }
            catch
            {
                return false;
            }

            return true;
        }

        private async Task<JwtSecurityToken> CreateJWTToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            foreach (var role in roles)
            {
                roleClaims.Add(new Claim("role", role));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub , user.Id ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti ,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email , user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Iss , "denation-app"),

            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jWT.SecurityKey));
            var signInCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

            var securityToken = new JwtSecurityToken(
                    issuer: _jWT.IssureIP,
                    audience : _jWT.AudienceIP,
                    claims : claims,
                    expires: DateTime.Now.AddDays(_jWT.DurationInDays),
                     signingCredentials: signInCredentials
                );

            return securityToken;
        }

        private static RefreshToken CreateRefreshToken()
        {
            var randomeNumber = RandomNumberGenerator.GetBytes(32);

            return new RefreshToken
            {
                CreatedOn = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                Token = Convert.ToBase64String(randomeNumber)
            };
        }

        private static OTPModel CreateOTP()
        {
            Random randomGenerator = new Random();
            var randomNumber = randomGenerator.Next(0, 1000000);

            return new OTPModel
            {
                OTP = randomNumber.ToString("000000"),
                ExpiresOn = DateTime.UtcNow.AddMinutes(5)
            };
        }
    }
}
