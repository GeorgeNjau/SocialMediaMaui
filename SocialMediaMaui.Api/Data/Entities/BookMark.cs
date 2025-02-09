namespace SocialMediaMaui.Api.Data.Entities
{
    public class BookMark
    {
        public Guid PostId { get; set; }
        public virtual Post Post { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }

}
