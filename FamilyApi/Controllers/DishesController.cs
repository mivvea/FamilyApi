using FamilyApi.DataModels;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace FamilyApi.Controllers
{
    [Route("[controller]")]
    public class DishesController : BaseController<Dish>
    {
        public DishesController(IMongoDatabase db)
            : base(db, "dishes") { }
    }
}