using ProjectRed.Core.DTOs.Data;
using ProjectRed.Core.DTOs.Requests.Auth;
using ProjectRed.Core.DTOs.Responses;

namespace ProjectRed.Core.Interfaces.Services.Auth
{
    public interface IRegisterService
    {
        Task<AuthResponse<UserDto>> RegisterLocalAsync(RegisterRequest request);
    }
}
