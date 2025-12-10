using AutoMapper;
using RecipesAPI.DTOs;
using RecipesAPI.Repositories;

namespace RecipesAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserProfileDTO?> GetUserProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            var profile = _mapper.Map<UserProfileDTO>(user);

            profile.RecipesCount = await _userRepository.GetRecipesCountAsync(userId);
            profile.FollowersCount = await _userRepository.GetFollowersCountAsync(userId);
            profile.FollowingCount = await _userRepository.GetFollowingCountAsync(userId);

            return profile;
        }

        public async Task<UserProfileDTO?> UpdateProfileAsync(Guid userId, UpdateProfileDTO dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            // Actualizar solo los campos proporcionados
            if (dto.Bio != null)
                user.Bio = dto.Bio;

            if (dto.ProfileImageUrl != null)
                user.ProfileImageUrl = dto.ProfileImageUrl;

            if (dto.BannerImageUrl != null)
                user.BannerImageUrl = dto.BannerImageUrl;

            await _userRepository.UpdateAsync(user);

            return await GetUserProfileAsync(userId);
        }

        public async Task<bool> DeleteAccountAsync(Guid userId)
        {
            return await _userRepository.DeleteAsync(userId);
        }

        public async Task<UserStatsDTO?> GetUserStatsAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            var stats = new UserStatsDTO
            {
                UserId = userId,
                RecipesCount = await _userRepository.GetRecipesCountAsync(userId),
                PublicRecipesCount = await _userRepository.GetPublicRecipesCountAsync(userId),
                PrivateRecipesCount = await _userRepository.GetPrivateRecipesCountAsync(userId),
                FollowersCount = await _userRepository.GetFollowersCountAsync(userId),
                FollowingCount = await _userRepository.GetFollowingCountAsync(userId),
                TotalFavoritesReceived = await _userRepository.GetTotalFavoritesReceivedAsync(userId)
            };

            return stats;
        }
    }
}
