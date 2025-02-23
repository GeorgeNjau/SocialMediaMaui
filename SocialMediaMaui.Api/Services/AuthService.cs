using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SocialMediaMaui.Api.Data;
using SocialMediaMaui.Api.Data.Entities;
using SocialMediaMaui.Shared.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SocialMediaMaui.Api.Services
{
    public class AuthService
    {
        private readonly DataContext context;
        private readonly IPasswordHasher<User> passwordHasher;
        private readonly PhotoUploadService photoUploadService;
        private readonly IConfiguration configuration;

        public AuthService(DataContext context, IPasswordHasher<User> passwordHasher, PhotoUploadService photoUploadService, IConfiguration configuration)
        {
            this.context = context;
            this.passwordHasher = passwordHasher;
            this.photoUploadService = photoUploadService;
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
                var (photoPath, photoUrl) = await photoUploadService.SaveUserPhotoAsync(photo, "uploads", "images", "users");
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

        public async Task<ApiResult<LoginResponseDto>> LoginAsync(LoginDto loginDto)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user is null)
                return ApiResult<LoginResponseDto>.Fail("User does not exist");

            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);

            if (passwordVerificationResult != PasswordVerificationResult.Success)
                return ApiResult<LoginResponseDto>.Fail("Invalid credentials provided");

            var jwt = GenerateJwtToken(user);

            var loggedInUser = new LoggedInUser(user.Id, user.Name, user.Name, user.PhotoUrl);
            var loginResponse = new LoginResponseDto(loggedInUser, jwt);

            return ApiResult<LoginResponseDto>.Success(loginResponse);
        }
             
        private string GenerateJwtToken(User user)
        {
            Claim[] claims = [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserPhotoUrl", user.PhotoUrl ?? "")
                ];

            var secretKey = configuration.GetValue<string>("Jwt:SecretKey");
            var securityKey = System.Text.Encoding.UTF8.GetBytes(secretKey);
            var symetricKey = new SymmetricSecurityKey(securityKey);
            var signinCredentials = new SigningCredentials(symetricKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(signingCredentials: signinCredentials,
                issuer: configuration.GetValue<string>("Jwt:Issuer"),
                expires: DateTime.Now.AddMinutes(configuration.GetValue<int>("Jwt:ExpireInMinutes")),
                claims: claims
                );

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token;

        }

    }
}
