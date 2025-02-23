using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialMediaMaui.Api.Data.Migrations
{
	/// <inheritdoc />
	public partial class GetUserPostsBookmarkPostsProc : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"CREATE OR ALTER PROCEDURE GetUserPosts
								(
									@StartIndex INT,
									@PageSize INT,
									@CurrentUserId UNIQUEIDENTIFIER
								)
								AS
								BEGIN
									SELECT 
										P.Id AS PostID,
										p.UserId,
										u.Name as UserName,
										u.PhotoUrl as UserPhotoUrl,
										p.Content,
										p.PhotoUrl,
										p.PostDate as PostedOn,
										p.ModifiedOn,
										CASE WHEN l.UserId IS NOT NULL THEN 1 ELSE 0 END AS IsLiked,
										CASE WHEN b.UserId IS NOT NULL THEN 1 ELSE 0 END AS IsBookMarked
									from posts p
									INNER JOIN Users u on p.UserId = u.Id
									left join likes l on p.id = l.PostId and l.UserId = @CurrentUserId
									left join BookMarks b on p.id = b.PostId and b.UserId = @CurrentUserId
									WHERE p.UserId = @CurrentUserId
									order by Coalesce(p.ModifiedOn, p.PostDate) desc
									OFFSET @StartIndex Rows
									FETCH NEXT @PageSize Rows ONLY

								END");

			migrationBuilder.Sql(@"CREATE OR ALTER PROCEDURE GetUserBookmarkedPosts
								(
									@StartIndex INT,
									@PageSize INT,
									@CurrentUserId UNIQUEIDENTIFIER
								)
								AS
								BEGIN
									SELECT 
										P.Id AS PostID,
										p.UserId,
										u.Name as UserName,
										u.PhotoUrl as UserPhotoUrl,
										p.Content,
										p.PhotoUrl,
										p.PostDate as PostedOn,
										p.ModifiedOn,
										CASE WHEN l.UserId IS NOT NULL THEN 1 ELSE 0 END AS IsLiked,
										CASE WHEN b.UserId IS NOT NULL THEN 1 ELSE 0 END AS IsBookMarked
									from posts p
									INNER JOIN Users u on p.UserId = u.Id
									left join likes l on p.id = l.PostId and l.UserId = @CurrentUserId
									left join BookMarks b on p.id = b.PostId and b.UserId = @CurrentUserId
									WHERE b.UserId = @CurrentUserId
									order by Coalesce(p.ModifiedOn, p.PostDate) desc
									OFFSET @StartIndex Rows
									FETCH NEXT @PageSize Rows ONLY

								END");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"DROP PROCEDURE GetUserPosts");
			migrationBuilder.Sql(@"DROP PROCEDURE GetUserBookmarkedPosts");
		}
	}
}
