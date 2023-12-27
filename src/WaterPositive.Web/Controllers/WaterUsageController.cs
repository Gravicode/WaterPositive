using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceStack.Redis;
using WaterPositive.Models;
using WaterPositive.Web.Data;

namespace WaterPositive.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WaterUsageController : ControllerBase
    {
        RedisManagerPool pool;
        WaterUsageService service;
        public WaterUsageController(WaterUsageService service, RedisManagerPool manager)
        {
            this.pool = manager;
            this.service = service;
        }
        // /api/Sms/GetData
        [AllowAnonymous]
        [HttpPost("[action]")]
        public IActionResult SyncData(List<WaterUsage> items)
        {
            try
            {
                foreach (var item in items)
                {
                    item.Id = 0; //so it will be treat as new object

                }
                var res = service.InsertDatas(items);
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("DeleteData")]
        public IActionResult DeleteData(int id)
        {
            var respon = service.DeleteData(id);
            if (respon == true)
            {
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpGet("FindByKeyword")]
        public IActionResult FindByKeyword(string Keyword)
        {
            var datas = service.FindByKeyword(Keyword);
            return Ok(datas);
        }
        [HttpGet("GetAllData")]
        public IActionResult GetAllData()
        {
            var datas = service.GetAllDataNoInclude();
            return Ok(datas);
        }
        [HttpGet("GetDataById")]
        public IActionResult GetDataById(int id)
        {
            var datas = service.GetDataById(id);
            return Ok(datas);
        }
        [HttpPost("InsertData")]
        public IActionResult InsertData([FromForm] WaterUsage data)
        {
            var datas = service.InsertData(data);
            return Ok(datas);
        }
        [HttpPut("UpdateData")]
        public IActionResult UpdateData([FromForm] WaterUsage data)
        {
            var respon = service.UpdateData(data);
            if (respon == true)
            {
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpGet("GetLastId")]
        public IActionResult GetLastId()
        {
            var datas = service.GetLastId();
            return Ok(datas);
        }

    }
}
