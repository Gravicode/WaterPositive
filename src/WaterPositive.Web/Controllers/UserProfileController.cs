using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WaterPositive.Models;
using WaterPositive.Web.Data;

namespace WaterPositive.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        UserProfileService service;
        public UserProfileController(UserProfileService service)
        {
            this.service = service;
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
            var datas = service.GetAllData();
            return Ok(datas);
        }
        [HttpGet("GetDataById")]
        public IActionResult GetDataById(int id)
        {
            var datas = service.GetDataById(id);
            return Ok(datas);
        }
        [HttpPost("InsertData")]
        public IActionResult InsertData([FromForm] UserProfile data)
        {
            var datas = service.InsertData(data);
            return Ok(datas);
        }
        [HttpPut("UpdateData")]
        public IActionResult UpdateData([FromForm] UserProfile data)
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
