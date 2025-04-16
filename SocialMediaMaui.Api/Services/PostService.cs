using Microsoft.EntityFrameworkCore;
using SocialMediaMaui.Api.Data;
using SocialMediaMaui.Api.Data.Entities;
using SocialMediaMaui.Shared.Dtos;

namespace SocialMediaMaui.Api.Services
{
    public class PostService
    {
        private readonly DataContext context;
        private readonly PhotoUploadService photoUploadService;

        public PostService(DataContext context, PhotoUploadService photoUploadService)
        {
            this.context = context;
            this.photoUploadService = photoUploadService;
        }

        public async Task<ApiResult> SavePostAsync(SavePostDto savePostDto, Guid userId)
        {
            string? existingPhotoPath = null;

            if (savePostDto.PostId == default)
            {
                // New 
                var post = new Post
                {
                    Content = savePostDto.Content,
                    PostDate = DateTime.UtcNow,
                    UserId = userId
                };

                // Save image if provided.
                if (savePostDto.Photo != null)
                {
                    (post.PhotoPath, post.PhotoUrl) = await photoUploadService.SaveUserPhotoAsync(savePostDto.Photo, "uploads", "images", "users", userId.ToString(), "posts");
                }

                context.Posts.Add(post);
            }
            else
            {
                // Existing
                var post = await context.Posts.FindAsync(savePostDto.PostId);

                if (post is null)
                    return ApiResult.Fail("Post no longer exists");

                if (post.UserId != userId)
                    return ApiResult.Fail("Permission denied. You cannot edit this post");

                post.Content = savePostDto.Content;
                post.ModifiedOn = DateTime.UtcNow;

                if (savePostDto.Photo is not null)
                {
                    existingPhotoPath = post.PhotoPath;

                    (post.PhotoPath, post.PhotoUrl) = await photoUploadService.SaveUserPhotoAsync(savePostDto.Photo, "uploads", "images", "users", userId.ToString(), "posts");
                }
                else
                {
                    if (savePostDto.ExistingPhotoRemoved)
                    {
                        existingPhotoPath = post.PhotoPath;
                        post.PhotoPath = null;
                        post.PhotoUrl = null;
                    }
                }

                context.Posts.Update(post);
            }

            try
            {
                await context.SaveChangesAsync();

                if (!string.IsNullOrWhiteSpace(existingPhotoPath) && File.Exists(existingPhotoPath))
                {
                    File.Delete(existingPhotoPath);
                }

                return ApiResult.Success();
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(ex.Message);
            }
        }

        //public async Task<PostDto[]> GetPostDtosAsync(int startIndex, int pageSize, Guid currentUserId)
        //{
        //    var posts = await context.Posts
        //        .Include(p => p.User)
        //        .Where(c => c.IsDelete == false)
        //        .OrderByDescending(b => b.PostDate)
        //        .Select(p => new PostDto
        //        {
        //            Content = p.Content,
        //            ModifiedOn = p.ModifiedOn,
        //            PhotoUrl = p.PhotoUrl,
        //            PostId = p.Id,
        //            PostedOn = p.PostDate,
        //            UserId = currentUserId,
        //            UserName = p.User.Name,
        //            UserPhotoUrl = p.User.PhotoUrl
        //        })
        //        .Skip(startIndex)
        //        .Take(pageSize).ToArrayAsync();

        //    var postIds = posts.Select(p => p.PostId).ToArray();
        //    var postsLikedByThisUser = await context.Likes
        //                                .Where(l => l.UserId == currentUserId && postIds.Contains(l.PostId))
        //                                .Select(l=> l.PostId)
        //                                .ToArrayAsync();

        //    var postsSavedByThisUser = await context.BookMarks
        //                              .Where(l => l.UserId == currentUserId && postIds.Contains(l.PostId))
        //                              .Select(l => l.PostId)
        //                              .ToArrayAsync();

        //    foreach (var post in posts)
        //    {
        //        post.IsBookMarked = postsSavedByThisUser.Contains(post.PostId);
        //        post.IsLiked = postsLikedByThisUser.Contains(post.PostId);
        //    }

        //    return posts;
        //}


        public async Task<PostDto[]> GetPostDtosAsync(int startIndex, int pageSize, Guid currentUserId)
        {
           var posts = await context.Set<PostDto>()
                .FromSqlInterpolated($"EXEC GetPosts @StartIndex = {startIndex}, @PageSize={pageSize}, @CurrentUserId = {currentUserId}")
                .ToArrayAsync();

            return posts;
        }

        public async Task<ApiResult<CommentDto>> SaveCommentAsync(SaveCommentDto dto, LoggedInUser currentUser)
        {
            Comment? comment = null;

            if (dto.CommentId == Guid.Empty)
            {
                comment = new Comment { Content = dto.Content, AddedOn = DateTime.UtcNow, PostId = dto.PostId, UserId = currentUser.Id };
                context.Comments.Add(comment);
            }
            else
            {
                comment = await context.Comments.FindAsync(dto.CommentId);

                if (comment is null)
                    return ApiResult<CommentDto>.Fail("Comment not found");

                if (comment.UserId != currentUser.Id)
                    return ApiResult<CommentDto>.Fail("You can modify your own comments only.");

                comment.Content = dto.Content;

                context.Comments.Update(comment);
            }

            try
            {
                await context.SaveChangesAsync();

                var commentDto = new CommentDto
                {
                    AddedOn = comment.AddedOn,
                    CommentId = comment.Id,
                    Content = comment.Content,
                    PostId = comment.PostId,
                    UserId = currentUser.Id,
                    UserName = currentUser.Name,
                    UserPhotoUrl = currentUser.PhotoUrl
                };

                return ApiResult<CommentDto>.Success(commentDto);
            }
            catch (Exception ex)
            {
                return ApiResult<CommentDto>.Fail(ex.Message);
            }

        }

        public async Task<CommentDto[]> GetPostCommentsAsync(Guid postId, int startIndex, int pageSize) =>
        
            await context.Comments.Where(c => c.PostId == postId)
                .OrderByDescending(c => c.AddedOn)
                .Skip(startIndex)
                .Take(pageSize)
                .Select(c => new CommentDto
                {
                    Content = c.Content,
                    AddedOn = c.AddedOn,
                    CommentId = c.Id,
                    PostId = c.PostId,
                    UserId = c.UserId,
                    UserName = c.User.Name,
                    UserPhotoUrl = c.User.PhotoUrl
                })
                .ToArrayAsync();

        public async Task<ApiResult> ToggleLikeAsync(Guid postId, Guid currentUserId)
        {
            var postExists = await context.Posts.AnyAsync(p => p.Id == postId);

            if (!postExists)
                return ApiResult.Fail("Post not found");

            try
            {
                var like = await context.Likes.FirstOrDefaultAsync(c => c.PostId == postId && c.UserId == currentUserId);

                if(like is null)
                {
                    like = new Like
                    {
                         PostId = postId,
                          UserId = currentUserId
                    };

                    context.Likes.Add(like);
                }
                else
                {
                    context.Likes.Remove(like);
                }

                await context.SaveChangesAsync();

                return ApiResult.Success();
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(ex.Message);
            }

        }

        public async Task<ApiResult> ToggleBookMarkAsync(Guid postId, Guid currentUserId)
        {
            var postExists = await context.Posts.AnyAsync(p => p.Id == postId);

            if (!postExists)
                return ApiResult.Fail("Post not found");

            try
            {
                var bookMark = await context.BookMarks.FirstOrDefaultAsync(c => c.PostId == postId && c.UserId == currentUserId);

                if (bookMark is null)
                {
                    bookMark = new BookMark
                    {
                        PostId = postId,
                        UserId = currentUserId
                    };

                    context.BookMarks.Add(bookMark);
                }
                else
                {
                    context.BookMarks.Remove(bookMark);
                }

                await context.SaveChangesAsync();

                return ApiResult.Success();
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(ex.Message);
            }

        }

        public async Task<ApiResult> DeletePostAsybc(Guid postId, Guid currentUserId)
        {
            try
            {
                var post = await context.Posts.FindAsync(postId);

                if (post is null)
                    return ApiResult.Fail("Post not found");

                if (post.UserId != currentUserId)
                    return ApiResult.Fail("You can delete your own posts only");

                context.Posts.Remove(post);

                await context.SaveChangesAsync();

                return ApiResult.Success();
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(ex.Message);
            }
        }


    }
}
