using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialMediaMaui.Api.Data;
using SocialMediaMaui.Api.Data.Entities;
using SocialMediaMaui.Shared.Dtos;

namespace SocialMediaMaui.Api.Services
{
    public class AuthService
    {
        private readonly DataContext context;
        private readonly IPasswordHasher<User> passwordHasher;
        private readonly IWebHostEnvironment hostEnvironment;
        private readonly IConfiguration configuration;

        public AuthService(DataContext context, IPasswordHasher<User> passwordHasher, IWebHostEnvironment hostEnvironment,
            IConfiguration configuration)
        {
            this.context = context;
            this.passwordHasher = passwordHasher;
            this.hostEnvironment = hostEnvironment;
            this.configuration = configuration;
        }

        public async Task<ApiResult<Guid>> RegisterUserAsync(RegisterDto registerDto)
        {
            if (await context.Users.AnyAsync(c => c.Email == registerDto.Email))
                return ApiResult<Guid>.Fail("User Exists");

            try
            {
                var user = new User
                {
                    Email = registerDto.Email,
                    Name = registerDto.name
                };

                user.PasswordHash = passwordHasher.HashPassword(user, registerDto.Password);

                context.Users.Add(user);
                await context.SaveChangesAsync();

                return ApiResult<Guid>.Success(user.Id);
            }
            catch (Exception ex)
            {
                return ApiResult<Guid>.Fail(ex.Message);
            }
        }

        public async Task<ApiResult> UploadPhotoAsync(Guid userId, IFormFile photo)
        {
            var user = await context.Users.FindAsync(userId);

            if (user is null)
                return ApiResult.Fail("User does not exist");

            try
            {
                var (photoPath, photoUrl) = await SaveUserPhotoAsync(photo);
                user.PhotoPath = photoPath;
                user.PhotoUrl = photoUrl;

                context.Users.Update(user);

                await context.SaveChangesAsync();

                return ApiResult.Success();
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(ex.Message);
            }
        }

        private async Task<(string PhotoPath, string PhotoUrl)> SaveUserPhotoAsync(IFormFile photo)
        {
            var targetFolder = Path.Combine(hostEnvironment.WebRootPath, "uploads", "images", "users");

            if(!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            var extension = Path.GetExtension(photo.FileName);
            var newPhotoName = $"{Guid.NewGuid()}_{DateTime.UtcNow.Ticks}{extension}";

            var fullPhotoPath = Path.Combine(targetFolder, newPhotoName);

            using FileStream fs = File.Create(fullPhotoPath);
            await photo.CopyToAsync(fs);

            var domainUrl = configuration.GetValue<string>("Domain").TrimEnd('/');

            var photoUrl = $"{domainUrl}/uploads/images/users/{newPhotoName}";

            return (fullPhotoPath, photoUrl);
        }

    }
}
