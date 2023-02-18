using WaterPositive.Models;
using Microsoft.AspNetCore.Mvc;
using ServiceStack.Redis;
using SkiaSharp;
using WaterPositive.Web.Data;

namespace WaterPositive.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WaterUsageController : Controller
    {
        WaterUsageService UsageSvc;
        RedisManagerPool pool;
        public WaterUsageController(RedisManagerPool manager, WaterUsageService service)
        {
            this.UsageSvc = service;
            this.pool = manager;


        }
        // /api/Sms/GetData
        [HttpPost("[action]")]
        public IActionResult SyncData(List<WaterUsage> items)
        {
            try
            {
                foreach (var item in items)
                {
                    item.Id = 0; //so it will be treat as new object
                    
                }
                var res = UsageSvc.InsertDatas(items);
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
    }

}
