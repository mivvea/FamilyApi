using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace FamilyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class FileController : ControllerBase
    {
        private readonly string _uploadFolder;

        public FileController()
        {
            _uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            Directory.CreateDirectory(_uploadFolder);
        }
        [HttpGet]
        public async Task<IActionResult> GetVideo()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "hello.mp4");

            if (!System.IO.File.Exists(path))
                return NotFound();

            var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(stream, "video/mp4", enableRangeProcessing: true);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".mp4", ".mov" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(ext))
                return BadRequest("Unsupported file type");

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(_uploadFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { fileName });
        }

        [HttpGet("{fileName}")]
        public IActionResult Get(string fileName)
        {
            var path = Path.Combine(_uploadFolder, fileName);

            if (!System.IO.File.Exists(path))
                return NotFound();

            var contentType = GetContentType(path);

            if (contentType.StartsWith("video"))
            {
                var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(stream, contentType, enableRangeProcessing: true);
            }

            return PhysicalFile(path, contentType);
        }

        private static string GetContentType(string path) => Path.GetExtension(path).ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".mp4" => "video/mp4",
            ".mov" => "video/quicktime",
            _ => "application/octet-stream"
        };
    }
}
