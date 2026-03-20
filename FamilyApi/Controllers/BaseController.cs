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
    public class BaseController<T> : ControllerBase where T : BaseItem
    {
        protected readonly IMongoCollection<T> _collection;

        public BaseController(IMongoDatabase database, string collectionName)
        {
            _collection = database.GetCollection<T>(collectionName);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _collection.Find(new BsonDocument()).ToListAsync();
            return Ok(items);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMy()
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized();

            var filter = Builders<T>.Filter.Eq(x => x.AddedBy, userName);
            var items = await _collection.Find(filter).ToListAsync();

            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] T item)
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized();

            item.AddedBy = userName;
            await _collection.InsertOneAsync(item);

            return Ok(item);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized();

            var filter =  Builders<T>.Filter.Eq(x => x.Id, ObjectId.Parse(id));
            var result = await _collection.DeleteOneAsync(filter);
            var existingItem = await _collection.Find(filter).FirstOrDefaultAsync();

            if (existingItem == null)
                return NotFound();

            if (!string.IsNullOrEmpty(existingItem.Photo))
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), existingItem.Photo);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }

            if (result.DeletedCount == 0)
                return NotFound();

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(string id, [FromBody] T updatedItem)
        {
            var filter = Builders<T>.Filter.Eq(x => x.Id, ObjectId.Parse(id));
            var existingItem = await _collection.Find(filter).FirstOrDefaultAsync();

            if (existingItem == null)
                return NotFound();

            updatedItem.Id = existingItem.Id;
            updatedItem.AddedBy = existingItem.AddedBy;

            await _collection.ReplaceOneAsync(filter, updatedItem);
            return Ok(updatedItem);
        }

        [HttpGet("random")]
        public async Task<IActionResult> Random()
        {
            var pipeline = new[]
            {
            new BsonDocument("$sample", new BsonDocument("size", 1))
        };

            var item = await _collection.Aggregate<T>(pipeline).FirstOrDefaultAsync();

            return Ok(item);
        }
    }
}
