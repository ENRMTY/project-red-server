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

        public async Task<AuthResponse<UserDto>> RegisterLocalAsync(RegisterRequest request)
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
                // check if user profile exists
                if (existingAuth.User != null && existingAuth.UserId != null)
                {
                    return new AuthResponse<UserDto>
                    {
                        Success = false,
                        Message = $"An account with this email already exists via {existingAuth.Provider}. You can log in with {existingAuth.Provider} or set a new password.",
                        Details = new UserDto
                        {
                            Id = existingAuth.UserId.Value,
                            DisplayName = existingAuth.User.DisplayName ?? existingAuth.User.Username,
                            Username = existingAuth.User.Username,
                            Email = existingAuth.Email
                        }
                    };
                }

                // user auth exists but profile is missing
                return new AuthResponse<UserDto>
                {
                    Success = false,
                    Message = $"An account with this email already exists via {existingAuth.Provider}. Please complete your profile first.",
                    Details = null,
                    RequiresProfileCompletion = true
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
                Email = userAuth.NormalizedEmail,
            };

            return new AuthResponse<UserDto>
            {
                Success = true,
                Message = "Successfully registered",
                Token = "token",
                Details = userDto
            };
        }

        public async Task<AuthResponse<UserDto>> RegisterOrLoginGoogleAsync(GoogleAuthRequest request)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            var existingGoogle = await _userAuthRepository
                .FindByProviderAndProviderId("google", request.ProviderUserId);

            if (existingGoogle != null)
            {
                // user exists but may not have completed profile
                if (existingGoogle.User == null || existingGoogle.UserId == null)
                {
                    return new AuthResponse<UserDto>
                    {
                        Success = true,
                        Message = "Please complete your profile",
                        Token = "token",
                        RequiresProfileCompletion = true,
                        Details = null
                    };
                }

                // user exists and has profile, log them in
                return new AuthResponse<UserDto>
                {
                    Success = true,
                    Message = "Successfully logged in with Google",
                    Token = "token",
                    Details = new UserDto
                    {
                        Id = existingGoogle.UserId.Value,
                        DisplayName = existingGoogle.User.DisplayName ?? existingGoogle.User.Username,
                        Username = existingGoogle.User.Username,
                        Email = existingGoogle.Email
                    }
                };
            }

            var existingAuthByEmail = await _userAuthRepository
                .FindUserAuthByEmail(normalizedEmail);

            if (existingAuthByEmail != null)
            {
                var googleAuth = new UserAuth
                {
                    UserId = existingAuthByEmail.UserId,
                    Provider = "google",
                    ProviderUserId = request.ProviderUserId,
                    Email = request.Email,
                    NormalizedEmail = normalizedEmail
                };

                await _userAuthRepository.AddAsync(googleAuth);
                await _appRepository.SaveChangesAsync();

                if (existingAuthByEmail.User != null && existingAuthByEmail.UserId != null)
                {
                    return new AuthResponse<UserDto>
                    {
                        Success = true,
                        Message = "Successfully logged in with Google",
                        Token = "token",
                        Details = new UserDto
                        {
                            Id = existingAuthByEmail.UserId.Value,
                            DisplayName = existingAuthByEmail.User.DisplayName ?? existingAuthByEmail.User.Username,
                            Username = existingAuthByEmail.User.Username,
                            Email = existingAuthByEmail.Email
                        }
                    };
                }

                return new AuthResponse<UserDto>
                {
                    Success = true,
                    Message = "Please complete your profile",
                    Token = "token",
                    RequiresProfileCompletion = true,
                    Details = null
                };
            }

            // create new auth entry without user
            var pendingAuth = new UserAuth
            {
                Provider = "google",
                ProviderUserId = request.ProviderUserId,
                Email = request.Email,
                NormalizedEmail = normalizedEmail,
                UserId = null
            };

            await _userAuthRepository.AddAsync(pendingAuth);
            await _appRepository.SaveChangesAsync();

            // tell frontend to go to profile creation
            return new AuthResponse<UserDto>
            {
                Success = true,
                Message = "Please complete your profile",
                Token = "token",
                RequiresProfileCompletion = true,
                Details = null
            };
        }

        public async Task<AuthResponse<UserDto>> CompleteProfileAsync(CompleteProfileRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                throw new InvalidInputException("Username is required");
            }

            if (string.IsNullOrWhiteSpace(request.DisplayName))
            {
                throw new InvalidInputException("Display name is required");
            }

            string normalizedUsername = request.Username.Trim().ToLowerInvariant();
            string displayName = request.DisplayName.Trim();

            // find the pending auth
            var pendingAuth = await _userAuthRepository.FindByProviderAndProviderId(
                request.Provider,
                request.ProviderUserId
            );

            if (pendingAuth == null)
            {
                throw new NotFoundException("Pending authentication record not found");
            }

            if (pendingAuth.UserId != null)
            {
                throw new InvalidOperationException("Profile is already completed");
            }

            var usernameExists = await _userRepository.UsernameExists(normalizedUsername);
            if (usernameExists)
            {
                throw new AlreadyExistsException("This username is already taken");
            }

            // finally create the user
            var user = new User
            {
                Username = normalizedUsername,
                DisplayName = displayName,
                CountryCode = "ZA",
                BirthYear = request.BirthYear
            };

            // assign the user to pending auth
            pendingAuth.User = user;

            await _userRepository.AddAsync(user);
            await _userAuthRepository.UpdateAsync(pendingAuth);
            bool saved = await _appRepository.SaveChangesAsync();

            if (!saved)
            {
                return new AuthResponse<UserDto>
                {
                    Success = false,
                    Message = "Failed to complete profile"
                };
            }

            return new AuthResponse<UserDto>
            {
                Success = true,
                Message = "Profile completed successfully",
                Token = "token",
                Details = new UserDto
                {
                    Id = user.Id,
                    DisplayName= user.DisplayName,
                    Username = user.Username,
                    Email = pendingAuth.NormalizedEmail
                }
            };
        }
    }
}
