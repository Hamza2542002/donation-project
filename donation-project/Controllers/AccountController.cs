using donation_project.DTO;
using donation_project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace donation_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IEmailService _emailService;

        public AccountController(IAccountService accountService, IEmailService emailService)
        {
            _accountService = accountService;
            _emailService = emailService;
        }

        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAccount(DeleteAccountDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUSerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(currentUSerId != model.Id)
                return Unauthorized("");

            var result = await _accountService.DeleteAccountAsync(model);

            if (!result)
                return BadRequest("Faild To Delete The Account");

            return Ok("Account Deleted");
        }
    }
}
