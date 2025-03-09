using SocialMediaMaui.Api.Extensions;
using SocialMediaMaui.Api.Services;
using SocialMediaMaui.Shared.Dtos;
using System.Security.Claims;

namespace SocialMediaMaui.Api.EndPoints
{
    public static class UserEndPoints
    {
        public static IEndpointRouteBuilder MapUserEndPoints(this IEndpointRouteBuilder app)
        {
            var userGroup = app.MapGroup("/api/auth")
                .RequireAuthorization()
                .WithTags("User");


            userGroup.MapPost("/change-photo", async (IFormFile photo, UserService userService, ClaimsPrincipal principal) =>
                Results.Ok(await userService.ChangePhotoAsync(photo, principal.GetUserId())))
                .Produces<ApiResult>()
                .WithName("ChangePhoto");


            userGroup.MapGet("/posts", async (int startIndex, int pageSize, UserService userService, ClaimsPrincipal claimsPrinciple) =>
              Results.Ok(await userService.GetUserPostsAsync(startIndex, pageSize, claimsPrinciple.GetUserId())))
              .Produces<PostDto[]>()
              .WithName("GetUserPosts");

            userGroup.MapGet("/bookmarked-posts", async (int startIndex, int pageSize, UserService userService, ClaimsPrincipal claimsPrinciple) =>
              Results.Ok(await userService.GetUserBookmarkedPostsAsync(startIndex, pageSize, claimsPrinciple.GetUserId())))
              .Produces<PostDto[]>()
              .WithName("GetUserBookmarkedPosts");

            return app;
        }

    }


}
