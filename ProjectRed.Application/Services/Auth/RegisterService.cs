using ProjectRed.Application.Validators;
using ProjectRed.Core.DTOs.Data;
using ProjectRed.Core.DTOs.Requests.Auth;
using ProjectRed.Core.DTOs.Responses;
using ProjectRed.Core.Entities;
using ProjectRed.Core.Exceptions;
using ProjectRed.Core.Interfaces.Repositories;
using ProjectRed.Core.Interfaces.Services.Auth;
using ProjectRed.Core.Interfaces.Services.Validators;

namespace ProjectRed.Application.Services.Auth
{
    public class RegisterService(IUserRepository userRepository, IPasswordHasher passwordHasher,
        IPasswordValidator passwordValidator, IUserAuthRepository userAuthRepository,
        IAppRepository appRepository) : IRegisterService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly IPasswordValidator _passwordValidator = passwordValidator;
        private readonly IUserAuthRepository _userAuthRepository = userAuthRepository;
        private readonly IAppRepository _appRepository = appRepository;

        public async Task<AuthResponse<UserDto>> RegisterAsync(RegisterRequest request)
        {
            var normalizedEmail = request.Email?.Trim();
            if (string.IsNullOrEmpty(normalizedEmail))
            {
                throw new InvalidInputException("An email is required");
            }

            var normalizedDisplayName = request.DisplayName?.Trim();
            if (string.IsNullOrEmpty(request.DisplayName))
            {
                throw new InvalidInputException("Display name is required");
            }

            if (string.IsNullOrEmpty(request.Password))
            {
                throw new InvalidInputException("Password is required");
            }

            var normalizedUsername = request.Username?.Trim();
            if (string.IsNullOrEmpty(normalizedUsername))
            {
                throw new InvalidInputException("Username is required");
            }

            var (IsValid, Message) = YearValidator.ValidateYear(request.BirthYear);
            if (!IsValid)
            {
                return new AuthResponse<UserDto>
                {
                    Success = IsValid,
                    Message = Message
                };
            }

            normalizedEmail = normalizedEmail.ToLowerInvariant();
            normalizedUsername = normalizedUsername.ToLowerInvariant();

            var usernameExists = await _userRepository.UsernameExists(normalizedUsername);
            if (usernameExists)
            {
                throw new AlreadyExistsException("This username is already taken");
            }

            var localAuthExists = await _userRepository.UserEmailExists(normalizedEmail);
            if (localAuthExists)
            {
                throw new AlreadyExistsException("Email already exists");
            }

            var existingAuth = await _userAuthRepository.FindUserAuthByEmail(normalizedEmail);
            if (existingAuth != null && existingAuth.Provider != "local")
            {
                return new AuthResponse<UserDto>
                {
                    Success = false,
                    Message = $"An account with this email already exists via {existingAuth.Provider}. You can log in with {existingAuth.Provider} or set a new password.",
                    Details = new UserDto
                    {
                        Id = existingAuth.UserId,
                        DisplayName = existingAuth.User.DisplayName ?? existingAuth.User.Username,
                        Username = existingAuth.User.Username,
                        Email = existingAuth.Email
                    }
                };
            }

            bool isValidPassword = _passwordValidator.IsValid(request.Password);
            if (!isValidPassword)
            {
                return new AuthResponse<UserDto>
                {
                    Success = false,
                    Message = "Password is invalid"
                };
            }

            var hashedPassword = _passwordHasher.HashPassword(request.Password);

            var user = new User
            {
                DisplayName = normalizedDisplayName,
                Username = normalizedUsername,
                BirthYear = request.BirthYear,
                CountryCode = "ZA"
            };

            var userAuth = new UserAuth
            {
                Provider = "local",
                NormalizedEmail = normalizedEmail,
                Email = normalizedEmail,
                PasswordHash = hashedPassword,
                User = user
            };

            await _userRepository.AddAsync(user);
            await _userAuthRepository.AddAsync(userAuth);
            
            bool added = await _appRepository.SaveChangesAsync();
            if (!added)
            {
                return new AuthResponse<UserDto>
                {
                    Success = false,
                    Message = "No user has been added"
                };
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Username = user.Username,
                Email = userAuth.Email
            };

            return new AuthResponse<UserDto>
            {
                Success = true,
                Message = "Successfully registered",
                Token = "token",
                Details = userDto
            };
        }
    }
}
