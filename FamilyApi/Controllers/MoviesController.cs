using FamilyApi.DataModels;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace FamilyApi.Controllers
{
    [Route("[controller]")]
    public class MoviesController : BaseController<Movie>
    {
        public MoviesController(IMongoDatabase db)
            : base(db, "movies") { }
    }
}
