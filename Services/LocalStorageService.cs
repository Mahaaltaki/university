// في مجلد Services/LocalStorageService.cs
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;
using System.Threading.Tasks;
using kalamon_University.Interfaces;

namespace kalamon_University.Services
{
    public class LocalStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly string _storagePath;

        public LocalStorageService(IWebHostEnvironment env)
        {
            _env = env;
            // سنحفظ الملفات في مجلد "exported_files" داخل wwwroot
            _storagePath = Path.Combine(_env.WebRootPath, "exported_files");
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        public async Task<string> SaveFileAsync(byte[] fileContents, string fileName)
        {
            var filePath = Path.Combine(_storagePath, fileName);
            await File.WriteAllBytesAsync(filePath, fileContents);

            // نرجع رابطًا نسبيًا للملف
            return $"/exported_files/{fileName}";
        }

        public Task<(byte[] fileContents, string contentType)?> GetFileAsync(string fileName)
        {
            var filePath = Path.Combine(_storagePath, fileName);
            if (!File.Exists(filePath))
            {
                return Task.FromResult<(byte[] fileContents, string contentType)?>(null);
            }

            var fileBytes = File.ReadAllBytes(filePath);
            new FileExtensionContentTypeProvider().TryGetContentType(fileName, out var contentType);

            return Task.FromResult<(byte[] fileContents, string contentType)?>((fileBytes, contentType ?? "application/octet-stream"));
        }
    }
}