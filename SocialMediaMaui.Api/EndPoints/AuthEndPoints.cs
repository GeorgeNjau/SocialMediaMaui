using SocialMediaMaui.Api.Services;
using SocialMediaMaui.Shared.Dtos;
using System.Threading.Tasks;

namespace SocialMediaMaui.Api.EndPoints
{
    public static class AuthEndPoints
    {
        public static IEndpointRouteBuilder MapAuthEndPoints(this IEndpointRouteBuilder app)
        {
            var authGroup = app.MapGroup("/api/auth")
                            .WithTags("Auth");

            authGroup.MapPost("/register", async (RegisterDto dto, AuthService authService) =>
                Results.Ok(await authService.RegisterUserAsync(dto)))
                .Produces<ApiResult<Guid>>()
                .WithName("Auth-Register");

            authGroup.MapPost("/register/{userId:guid}", async (Guid userId, IFormFile photo, AuthService authService) =>
               Results.Ok(await authService.UploadPhotoAsync(userId, photo)))
               .Produces<ApiResult>()
               .WithName("Auth-AddPhoto-to-User");

            authGroup.MapPost("/login", async (LoginDto dto, AuthService authService) =>
                Results.Ok(await authService.LoginAsync(dto)))
                .Produces<ApiResult<LoginResponseDto>>()
                .WithName("Auth-Login");

            return app;
        }

    }


}
