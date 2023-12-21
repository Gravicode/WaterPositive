using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaterPositive.Web.Data;

namespace WaterPositive.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserApiController : ControllerBase
    {
        private IRestApiService _userService;

        public UserApiController(IRestApiService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] ServiceAuth auth)
        {
            var user = _userService.Authenticate(auth.APIKey);

            if (user == null)
                return BadRequest(new { message = "API KEY is incorrect" });

            return Ok(user);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }
    }
}
