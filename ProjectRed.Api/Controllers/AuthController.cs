using Microsoft.AspNetCore.Mvc;
using ProjectRed.Core.DTOs.Requests.Auth;
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
            try
            {
                var result = await _registerService.CompleteProfileAsync(request);

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
    }
}
