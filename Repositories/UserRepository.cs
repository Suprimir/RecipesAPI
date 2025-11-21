using Microsoft.EntityFrameworkCore;
using RecipesAPI.Data;
using RecipesAPI.Models;

namespace RecipesAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly RecipesDbContext _context;

        public UserRepository(RecipesDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> UpdateAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetRecipesCountAsync(Guid userId)
        {
            return await _context.Recipes
                .CountAsync(r => r.UserId == userId);
        }

        public async Task<int> GetPublicRecipesCountAsync(Guid userId)
        {
            return await _context.Recipes
                .CountAsync(r => r.UserId == userId && r.IsPublic);
        }

        public async Task<int> GetPrivateRecipesCountAsync(Guid userId)
        {
            return await _context.Recipes
                .CountAsync(r => r.UserId == userId && !r.IsPublic);
        }

        public async Task<int> GetFollowersCountAsync(Guid userId)
        {
            return await _context.Follows
                .CountAsync(f => f.FollowingId == userId);
        }

        public async Task<int> GetFollowingCountAsync(Guid userId)
        {
            return await _context.Follows
                .CountAsync(f => f.FollowerId == userId);
        }

        public async Task<int> GetTotalFavoritesReceivedAsync(Guid userId)
        {
            return await _context.Recipes
                .Where(r => r.UserId == userId)
                .SumAsync(r => r.FavoritesCount);
        }
    }
}
