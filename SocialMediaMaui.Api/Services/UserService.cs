using Microsoft.EntityFrameworkCore;
using SocialMediaMaui.Api.Data;
using SocialMediaMaui.Shared.Dtos;

namespace SocialMediaMaui.Api.Services
{
    public class UserService
    {
        private readonly DataContext dataContext;
        private readonly PhotoUploadService photoUploadService;

        public UserService(DataContext dataContext, PhotoUploadService photoUploadService)
        {
            this.dataContext = dataContext;
            this.photoUploadService = photoUploadService;
        }

        public async Task<ApiResult<string>> ChangePhotoAsync(IFormFile photo, Guid currentUserId)
        {
            var user = await dataContext.Users.FindAsync(currentUserId);

            if (user is null)
                return ApiResult<string>.Fail("User not found");

            try
            {
                var existingPhotoPath = user.PhotoPath;

                (user.PhotoPath, user.PhotoUrl) = await photoUploadService.SaveUserPhotoAsync(photo, "uploads", "images", "users");
                
                dataContext.Users.Update(user);
                await dataContext.SaveChangesAsync();

                return ApiResult<string>.Success(user.PhotoUrl);
            }
            catch (Exception ex)
            {
                return ApiResult<string>.Fail(ex.Message);
            }

        }

        public async Task<PostDto[]> GetUserPostsAsync(int startIndex, int pageSize, Guid currentUserId)
        {
            var posts = await dataContext.Set<PostDto>()
               .FromSqlInterpolated($"EXEC GetUserPosts @StartIndex = {startIndex}, @PageSize={pageSize}, @CurrentUserId = {currentUserId}")
               .ToArrayAsync();

            return posts;
        }

        public async Task<PostDto[]> GetUserBookmarkedPostsAsync(int startIndex, int pageSize, Guid currentUserId)
        {
            var posts = await dataContext.Set<PostDto>()
               .FromSqlInterpolated($"EXEC GetUserBookmarkedPosts @StartIndex = {startIndex}, @PageSize={pageSize}, @CurrentUserId = {currentUserId}")
               .ToArrayAsync();

            return posts;
        }

    }
}
