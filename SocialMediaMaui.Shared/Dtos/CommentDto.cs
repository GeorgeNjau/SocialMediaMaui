﻿namespace SocialMediaMaui.Shared.Dtos
{
    public class CommentDto
    {
        public Guid PostId { get; set; }
        public Guid CommentId { get; set; }

        public required string Content { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string? UserPhotoUrl { get; set; }
        public DateTime? AddedOn { get; set; }
    }

}
