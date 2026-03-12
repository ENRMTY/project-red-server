using ProjectRed.Core.DTOs.Data;
using ProjectRed.Core.DTOs.Requests.Auth;
using ProjectRed.Core.DTOs.Responses;
using ProjectRed.Core.Entities;
using ProjectRed.Core.Interfaces.Repositories;
using ProjectRed.Core.Interfaces.Services.Auth;
using System.Net.Mail;

namespace ProjectRed.Application.Services.Auth
{
    public class LoginService(IUserAuthRepository userAuthRepository, IPasswordHasher passwordHasher,
        ITokenService tokenService) : ILoginService
    {
        private readonly IUserAuthRepository _userAuthRepository = userAuthRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly ITokenService _tokenService = tokenService;

        public async Task<AuthResponse<UserDto>> LoginWithEmailOrUsernameAsync(LoginRequest request)
        {
            var normalizedIdentifier = request.Identifier.Trim().ToLowerInvariant();
            UserAuth? userAuth;

            if (IsValidEmail(normalizedIdentifier))
            {
                userAuth = await _userAuthRepository.FindUserAuthByEmail(normalizedIdentifier);
            }
            else
            {
                userAuth = await _userAuthRepository.FindUserAuthByUsername(normalizedIdentifier);
            }

            if (userAuth == null)
            {
                return new AuthResponse<UserDto>
                {
                    Success = false,
                    Message = "Invalid credentials"
                };
            }

            if (userAuth.User == null)
            {
                return new AuthResponse<UserDto>
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            if (userAuth.PasswordHash == null)
            {
                return new AuthResponse<UserDto>
                {
                    Success = false,
                    Message = "Invalid credentials"
                };
            }

            if (!_passwordHasher.VerifyPassword(request.Password, userAuth.PasswordHash))
            {
                return new AuthResponse<UserDto>
                {
                    Success = false,
                    Message = "Invalid credentials"
                };
            }

            var token = _tokenService.GenerateAuthToken(
                userId: userAuth.User.Id,
                email: userAuth.NormalizedEmail,
                username: userAuth.User.Username
            );

            return new AuthResponse<UserDto>
            {
                Success = true,
                Message = "Successfully logged in",
                Token = token,
                Details = new UserDto
                {
                    Id = userAuth.User.Id,
                    DisplayName = userAuth.User.DisplayName,
                    Username = userAuth.User.Username,
                    Email = userAuth.NormalizedEmail
                }
            };
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
