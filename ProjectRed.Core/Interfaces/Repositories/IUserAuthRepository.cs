using ProjectRed.Core.Entities;

namespace ProjectRed.Core.Interfaces.Repositories
{
    public interface IUserAuthRepository
    {
        Task AddAsync(UserAuth userAuth);
        Task<UserAuth?> FindUserAuthByEmail(string email);
        Task<bool> SaveChangesAsync();
    }
}
