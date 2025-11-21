using Microsoft.EntityFrameworkCore;
using RecipesAPI.Data;
using RecipesAPI.Models;

namespace RecipesAPI.Repositories
{
    public class FollowRepository : IFollowRepository
    {
        private readonly RecipesDbContext _context;

        public FollowRepository(RecipesDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Follow>> GetFollowersAsync(Guid userId, int page, int limit)
        {
            return await _context.Follows
                .Where(f => f.FollowingId == userId)
                .Include(f => f.Follower)
                .OrderByDescending(f => f.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<Follow>> GetFollowingAsync(Guid userId, int page, int limit)
        {
            return await _context.Follows
                .Where(f => f.FollowerId == userId)
                .Include(f => f.Following)
                .OrderByDescending(f => f.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<Follow?> GetFollowAsync(Guid followerId, Guid followingId)
        {
            return await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);
        }

        public async Task<Follow> CreateFollowAsync(Follow follow)
        {
            follow.CreatedAt = DateTime.UtcNow;
            _context.Follows.Add(follow);
            await _context.SaveChangesAsync();

            await _context.Entry(follow).Reference(f => f.Following).LoadAsync();
            return follow;
        }

        public async Task<bool> DeleteFollowAsync(Guid followerId, Guid followingId)
        {
            var follow = await GetFollowAsync(followerId, followingId);
            if (follow == null) return false;

            _context.Follows.Remove(follow);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsFollowingAsync(Guid followerId, Guid followingId)
        {
            return await _context.Follows
                .AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);
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
    }
}
