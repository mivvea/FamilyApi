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
    public class DishesController : ControllerBase
    {
        private readonly IMongoDatabase _mongoDatabase;
        private readonly IMongoCollection<Dish> _dishesCollection;
        public DishesController(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
            _dishesCollection = _mongoDatabase.GetCollection<Dish>("dishes");
        }
        [HttpGet(Name = "GetDishes")]

        public async Task<IActionResult> Get()
        {
            try
            {
                var dishes = await _dishesCollection.Find(new BsonDocument()).ToListAsync();
                return Ok(dishes);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
        [HttpGet("MyDishes")]
        public async Task<IActionResult> GetMyDishes()
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized("User ID not found in token.");
            }
            var filter = Builders<Dish>.Filter.Eq(d => d.AddedBy, userName);

            var dishes = await _dishesCollection.Find(filter).ToListAsync();

            if (dishes == null || dishes.Count == 0)
            {
                return NotFound("No dishes found for this user.");
            }

            return Ok(dishes);
        }
        [HttpPost]
        public async Task<ActionResult<Dish>> AddDish([FromBody] Dish newDish)
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized("User ID not found in token.");
            }
            try
            {
                var dishDocument = new Dish
                {
                     Name = newDish.Name,
                     Photo = newDish.Photo,
                     AddedBy = userName
                };

                await _dishesCollection.InsertOneAsync(dishDocument);

                return CreatedAtAction(nameof(AddDish), dishDocument);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}