namespace SocialMediaMaui.Shared.Dtos
{
    public class SaveCommentDto
    {
        public Guid PostId { get; set; }
        public Guid CommentId { get; set; }

        public required string Content { get; set; }

        public bool Validate() => string.IsNullOrWhiteSpace(Content);
    }

}
