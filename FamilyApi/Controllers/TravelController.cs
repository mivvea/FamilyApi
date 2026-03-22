using FamilyApi.DataModels;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace FamilyApi.Controllers
{
    [Route("[controller]")]
    public class TravelController : BaseController<Movie>
    {
        public TravelController(IMongoDatabase db)
            : base(db, "travel") { }
    }
}
