using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnSleepingEyeServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        AppDbContext db = new AppDbContext();
        [HttpGet("/User/Current")]
        [Authorize(AuthenticationSchemes =
                JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GeUserInfoByID(string id)
        {
            var result = db.GetUserByID(id);
            return new JsonResult(result);
        }
        [HttpGet("/User/Current/Roles")]
        [Authorize(AuthenticationSchemes =
        JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetUserRolesByID(string id)
        {
            var result = db.GetWorkerRolesByID(id);
            return new JsonResult(result);
        }
    }
}
