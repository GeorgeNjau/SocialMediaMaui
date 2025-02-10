using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaMaui.Api.Data.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(30)]
        public required string Name { get; set; }

        [EmailAddress]
        [MaxLength(150)]
        public required string Email { get; set; }

        [MaxLength(300)]
        [Required]
        public string PasswordHash { get; set; }

        [Comment("Physical path of the image.")] // To enable move or delete the image from the device
        public string? PhotoPath { get; set; }

        public string? PhotoUrl { get; set; } // To enable consumption by other apps

    }

}
