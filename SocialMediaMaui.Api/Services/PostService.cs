﻿using Microsoft.EntityFrameworkCore;
using SocialMediaMaui.Api.Data;
using SocialMediaMaui.Api.Data.Entities;
using SocialMediaMaui.Shared.Dtos;

namespace SocialMediaMaui.Api.Services
{
    public class PostService
    {
        private readonly DataContext context;
        private readonly IWebHostEnvironment hostEnvironment;
        private readonly IConfiguration configuration;

        public PostService(DataContext context, IWebHostEnvironment hostEnvironment, IConfiguration configuration)
        {
            this.context = context;
            this.hostEnvironment = hostEnvironment;
            this.configuration = configuration;
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
                    (post.PhotoPath, post.PhotoUrl) = await SavePostPhotoAsync(savePostDto.Photo, userId);
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

                    (post.PhotoPath, post.PhotoUrl) = await SavePostPhotoAsync(savePostDto.Photo, userId);
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

        private async Task<(string PhotoPath, string PhotoUrl)> SavePostPhotoAsync(IFormFile photo, Guid userId)
        {
            var targetFolder = Path.Combine(hostEnvironment.WebRootPath, "uploads", "images", "users", userId.ToString(), "posts");

            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            var extension = Path.GetExtension(photo.FileName);
            var newPhotoName = $"{Guid.NewGuid()}_{DateTime.UtcNow.Ticks}{extension}";

            var fullPhotoPath = Path.Combine(targetFolder, newPhotoName);

            using FileStream fs = File.Create(fullPhotoPath);
            await photo.CopyToAsync(fs);

            var domainUrl = configuration.GetValue<string>("Domain").TrimEnd('/');

            var photoUrl = $"{domainUrl}/uploads/images/users/{userId}/posts/{newPhotoName}";

            return (fullPhotoPath, photoUrl);
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



    }
}
