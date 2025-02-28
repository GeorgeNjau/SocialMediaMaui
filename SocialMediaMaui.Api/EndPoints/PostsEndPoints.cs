using SocialMediaMaui.Api.Extensions;
using SocialMediaMaui.Api.Services;
using SocialMediaMaui.Shared.Dtos;
using System.Security.Claims;

namespace SocialMediaMaui.Api.EndPoints
{
    public static class PostsEndPoints
    {
        public static async Task<IEndpointRouteBuilder> MapAuthEndPoints(this IEndpointRouteBuilder app)
        {
            var postsGroup = app.MapGroup("/api/posts")
                            .RequireAuthorization()
                            .WithTags("Posts");

            postsGroup.MapPost("/save-post", async (SavePostDto dto, PostService postService, ClaimsPrincipal principal) =>
                    Results.Ok(await postService.SavePostAsync(dto, principal.GetUserId())))
            .Produces<ApiResult>()
            .WithName("SavePost");

            postsGroup.MapGet("/", async (int startIndex, int pageSize, PostService postService, ClaimsPrincipal claimsPrinciple) =>
                Results.Ok(await postService.GetPostDtosAsync(startIndex, pageSize, claimsPrinciple.GetUserId())))
                .Produces<PostDto[]>()
                .WithName("GetPosts");

            postsGroup.MapPost("/{postId:guid}/comments", async (Guid postId, SaveCommentDto dto, PostService postService, ClaimsPrincipal principal) =>
                Results.Ok(await postService.SaveCommentAsync(dto, principal.GetUser())))
                .Produces<ApiResult<CommentDto>>()
                .WithName("SaveComment");

            postsGroup.MapGet("/{postId:guid}/comments", async (Guid postId, int startIndex, int pageSize, PostService postService) =>
               Results.Ok(await postService.GetPostCommentsAsync(postId, startIndex, pageSize)))
               .Produces<CommentDto[]>()
               .WithName("GetPostComments");

            postsGroup.MapPost("/{postId:guid}/toggle-like", async (Guid postId, PostService postService, ClaimsPrincipal principal) =>
              Results.Ok(await postService.ToggleLikeAsync(postId, principal.GetUserId())))
              .Produces<ApiResult>()
              .WithName("ToggleLike");

            postsGroup.MapPost("/{postId:guid}/toggle-bookmark", async (Guid postId, PostService postService, ClaimsPrincipal principal) =>
             Results.Ok(await postService.ToggleBookMarkAsync(postId, principal.GetUserId())))
             .Produces<ApiResult>()
             .WithName("ToggleBookmark");

            postsGroup.MapDelete("/{postId:guid}", async (Guid postId, PostService postService, ClaimsPrincipal principal) =>
            Results.Ok(await postService.DeletePostAsybc(postId, principal.GetUserId())))
                .Produces<ApiResult>()
                .WithName("DeletePost");

            return app;
        }
    }
}
