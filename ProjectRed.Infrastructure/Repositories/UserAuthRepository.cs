using Microsoft.EntityFrameworkCore;
using ProjectRed.Core.Entities;
using ProjectRed.Core.Interfaces.Repositories;
using ProjectRed.Infrastructure.Data;

namespace ProjectRed.Infrastructure.Repositories
{
    public class UserAuthRepository(AppDbContext dbContext) : IUserAuthRepository
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task AddAsync(UserAuth userAuth)
        {
            await _dbContext.UserAuths.AddAsync(userAuth);
        }

        public async Task UpdateAsync(UserAuth userAuth)
        {
            _dbContext.UserAuths.Update(userAuth);
            await Task.CompletedTask;
        }

        public async Task<UserAuth?> FindUserAuthByEmail(string email)
        {
            var existingAuth = await _dbContext.UserAuths
                .Include(ua => ua.User)
                .FirstOrDefaultAsync(ua => ua.NormalizedEmail == email);

            return existingAuth;
        }

        public async Task<UserAuth?> FindUserAuthByUsername(string username)
        {
            var existingAuth = await _dbContext.UserAuths
                .Include(ua => ua.User)
                .Where(ua => ua.User != null && ua.User.Username == username)
                .FirstOrDefaultAsync();

            return existingAuth;
        }

        public async Task<UserAuth?> FindByProviderAndProviderId(string provider, string providerUserId)
        {
            var existingAuth = await _dbContext.UserAuths
                .Include(ua => ua.User)
                .FirstOrDefaultAsync(ua =>
                    ua.Provider == provider &&
                    ua.ProviderUserId == providerUserId);

            return existingAuth;
        }

        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                var rows = await _dbContext.SaveChangesAsync();
                return rows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
