using ProjectRed.Core.Interfaces.Repositories;
using ProjectRed.Infrastructure.Data;

namespace ProjectRed.Infrastructure.Repositories
{
    public class AppRepository(AppDbContext dbContext) : IAppRepository
    {
        private readonly AppDbContext _dbContext = dbContext;

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
