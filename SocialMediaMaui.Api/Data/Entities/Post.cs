using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaMaui.Api.Data.Entities
{
    public class Post
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public string? Content { get; set; }

        [MaxLength(300)]
        public required string PasswordHash { get; set; }

        [Comment("Physical path of the image.")] // To enable move or delete the image from the device
        public string? PhotoPath { get; set; }

        public string? PhotoUrl { get; set; } // To enable consumption by other apps

        public DateTime PostDate { get; set; }

        public DateTime ModifiedOn { get; set; }

        public bool IsDelete { get; set; }


    }

}
