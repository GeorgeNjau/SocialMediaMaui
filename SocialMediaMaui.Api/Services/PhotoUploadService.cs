namespace SocialMediaMaui.Api.Services
{
    public class PhotoUploadService
    {
        private readonly IWebHostEnvironment hostEnvironment;
        private readonly IConfiguration configuration;

        public PhotoUploadService(IWebHostEnvironment hostEnvironment, IConfiguration configuration)
        {
            this.hostEnvironment = hostEnvironment;
            this.configuration = configuration;
        }

        public async Task<(string PhotoPath, string PhotoUrl)> SaveUserPhotoAsync(IFormFile photo, params string[] folderPaths)
        {
            var targetFolder = Path.Combine([hostEnvironment.WebRootPath, ..folderPaths]);

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

            var photoUrl = $"{domainUrl}/{string.Join('/', folderPaths)}/{newPhotoName}";

            return (fullPhotoPath, photoUrl);
        }
    }
}
