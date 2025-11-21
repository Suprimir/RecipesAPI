namespace RecipesAPI.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        public FileStorageService(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
        }

        public bool IsValidImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            if (file.Length > MaxFileSize)
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                return false;

            return true;
        }

        public async Task<string> SaveImageAsync(IFormFile file, string folder)
        {
            if (!IsValidImageFile(file))
                throw new Exception("Archivo de imagen inválido");

            // Crear directorio si no existe
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", folder);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Generar nombre único para el archivo
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Guardar archivo
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Retornar URL relativa
            return GetImageUrl(fileName, folder);
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                    return false;

                // Extraer la ruta relativa de la URL
                var uri = new Uri(imageUrl, UriKind.RelativeOrAbsolute);
                var relativePath = uri.IsAbsoluteUri ? uri.AbsolutePath : imageUrl;

                // Remover el prefijo /uploads/ si existe
                if (relativePath.StartsWith("/uploads/"))
                {
                    relativePath = relativePath.Substring("/uploads/".Length);
                }
                else if (relativePath.StartsWith("uploads/"))
                {
                    relativePath = relativePath.Substring("uploads/".Length);
                }

                var filePath = Path.Combine(_environment.WebRootPath, "uploads", relativePath);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public string GetImageUrl(string fileName, string folder)
        {
            var baseUrl = _configuration["AppSettings:BaseUrl"] ?? "http://localhost:5000";
            return $"{baseUrl}/uploads/{folder}/{fileName}";
        }
    }
}
