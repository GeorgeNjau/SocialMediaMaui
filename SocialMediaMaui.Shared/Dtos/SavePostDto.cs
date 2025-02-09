using Microsoft.AspNetCore.Http;

namespace SocialMediaMaui.Shared.Dtos
{
    public class SavePostDto
    {
        public Guid PostId { get; set; }
        public string? Content { get; set; }
        public IFormFile? Photo { get; set; }
        public string? ExistingPhoto { get; set; }

        public bool Validate() => !string.IsNullOrWhiteSpace(Content) || Photo is not null;
    }

}
