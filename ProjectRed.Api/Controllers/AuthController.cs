using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectRed.Core.DTOs.Requests.Auth;
using ProjectRed.Core.Enums;
using ProjectRed.Core.Interfaces.Services.Auth;

namespace ProjectRed.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IRegisterService registerService) : ControllerBase
    {
        private readonly IRegisterService _registerService = registerService;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var result = await _registerService.RegisterLocalAsync(request);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("google")]
        public async Task<IActionResult> RegisterOrLoginGoogle([FromBody] GoogleAuthRequest request)
        {
            try
            {
                var result = await _registerService.RegisterOrLoginGoogleAsync(request);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("complete-profile")]
        public async Task<IActionResult> CompleteProfile([FromBody] CompleteProfileRequest request)
        {
            var tokenType = User.FindFirst(TokenType.ProfileCompletion.ToString().ToLower())?.Value;

            if (tokenType != TokenType.ProfileCompletion.ToString().ToLower())
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid token type"
                });
            }

            var provider = User.FindFirst("provider")?.Value;
            var providerUserId = User.FindFirst("provider_user_id")?.Value;

            if (provider == null || providerUserId == null)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid token"
                });
            }

            try
            {
                var result = await _registerService.CompleteProfileAsync(request, provider, providerUserId);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // test method
        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var userId = User.FindFirst("sub")?.Value;
            var email = User.FindFirst("email")?.Value;
            var tokenType = User.FindFirst("token_type")?.Value;

            return Ok(new { userId, email, tokenType });
        }
    }
}
