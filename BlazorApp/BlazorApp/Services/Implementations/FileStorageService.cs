using BlazorApp.Services.Interfaces;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorApp.Services.Implementations
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _uploadPath;

        public FileStorageService(IWebHostEnvironment env)
        {
            _uploadPath = Path.Combine(env.WebRootPath, "files");
            Directory.CreateDirectory(_uploadPath);
        }

        public async Task<string> StoreAsync(IBrowserFile file)
        {
            if (file == null || file.Size == 0) return string.Empty;

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.Name)}";
            var fullPath = Path.Combine(_uploadPath, fileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024).CopyToAsync(stream); // 10MB limit

            return fileName;
        }
    }
}
