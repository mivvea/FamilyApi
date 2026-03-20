using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace FamilyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class VideoController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetVideo()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "hello.MP4");

            if (!System.IO.File.Exists(path))
                return NotFound();

            var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(stream, "video/mp4", enableRangeProcessing: true);
        }
    }
}
