using FamilyApi.DataModels;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace FamilyApi.Controllers
{
    [Route("[controller]")]
    public class HistoryController : BaseController<History>
    {
        public HistoryController(IMongoDatabase db)
            : base(db, "history") { }
    }
}