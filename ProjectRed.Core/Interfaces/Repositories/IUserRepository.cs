using ProjectRed.Core.Entities;

namespace ProjectRed.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> FindById(int id);
        Task<User?> FindByEmail(string email);
        Task<bool> UsernameExists(string username);
        Task<bool> UserEmailExists(string email);
        Task AddAsync(User user);
        Task<bool> SaveChangesAsync();
    }
}
