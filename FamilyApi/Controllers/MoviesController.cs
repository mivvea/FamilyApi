using FamilyApi.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Claims;

namespace FamilyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class MoviesController : ControllerBase
    {
        private readonly IMongoDatabase _mongoDatabase;
        private readonly IMongoCollection<Movie> _moviesCollection;
        public MoviesController(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
            _moviesCollection = _mongoDatabase.GetCollection<Movie>("movies");
        }
        [HttpGet(Name = "GetMovies")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var movies = await _moviesCollection.Find(new BsonDocument()).ToListAsync();
                return Ok(movies);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("MyMovies")]
        public async Task<IActionResult> GetMyMovies()
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized("User ID not found in token.");
            }
            var filter = Builders<Movie>.Filter.Eq(d => d.AddedBy, userName);

            var movies = await _moviesCollection.Find(filter).ToListAsync();

            if (movies == null || movies.Count == 0)
            {
                return NotFound("No movies found for this user.");
            }

            return Ok(movies);
        }

        [HttpPost]
        public async Task<ActionResult<Dish>> AddMovie([FromBody] Movie newMovie)
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized("User ID not found in token.");
            }
            try
            {
                newMovie.AddedBy = userName;
                await _moviesCollection.InsertOneAsync(newMovie);

                return CreatedAtAction(nameof(AddMovie), newMovie);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
