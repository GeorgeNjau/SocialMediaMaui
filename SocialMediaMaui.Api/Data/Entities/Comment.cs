﻿using System.ComponentModel.DataAnnotations;

namespace SocialMediaMaui.Api.Data.Entities
{
    public class Comment
    {
        [Key]
        public Guid Id { get; set; }

        public Guid PostId { get; set; }
        public virtual Post Post { get; set; }

        public required string Content { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public DateTime AddedOn { get; set; }
    }

}
