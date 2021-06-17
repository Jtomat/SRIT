using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UnSleepingEyeServer.Support;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace UnSleepingEyeServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        AppDbContext db = new AppDbContext();
        [HttpGet("/Project")]
        [Authorize(AuthenticationSchemes =
                JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Project(int id)
        {
            var result = db.GetProjectByID(id);
            return new JsonResult(result);
        }
        [HttpGet("/Projects")]
        [Authorize(AuthenticationSchemes =
        JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult ProjectsAll()
        {
            var result = db.GetAllProjects();
            return new JsonResult(result);
        }
        [HttpGet("/User/Task")]
        [Authorize(AuthenticationSchemes =
            JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult TasksFor(string id)
        {
            var user = db.GetTasksWithPriorityFor(id);
            return new JsonResult(user);
        }
        [HttpGet("/Project/Stages")]
        [Authorize(AuthenticationSchemes =
            JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult StagesOf(int id) 
        {
            var project = db.GetProjectByID(id);
            return new JsonResult(project.Stage);
        }
        [HttpGet("/Project/Stages/Tasks")]
        [Authorize(AuthenticationSchemes =
    JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult StagesTasks(int id)
        {
            var project = db.GetTasksForStage(id);
            return project != null ? new JsonResult(project.Stage) : new JsonResult(null);
        }
        [HttpPost("/Project/Add")]
        [Authorize(AuthenticationSchemes =
            JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult AddProject(string name, string info)
        {
            var project = db.CreateProject(name, info);
            return new JsonResult(project);
        }
        [HttpPost("/Project/Edit")]
        [Authorize(AuthenticationSchemes =
   JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult AddProject(long id, string name, string info)
        {
            var project = db.updProject(id, name, info);
            return new JsonResult(project);
        }
        [HttpPost("/Project/Stages/Remove")]
        [Authorize(AuthenticationSchemes =
            JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult StagesRemove(long id)
        {
            var project = db.RemoveStage(id);
            return new JsonResult(project);
        }
        [HttpPost("/Project/Stages/Update")]
        [Authorize(AuthenticationSchemes =
            JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult UpdStage(long id, string name, string dateS, string dateE)
        {
            var project = db.UpdStage(id, name, dateS, dateE);
            return new JsonResult(project);
        }
        [HttpGet("/Project/Remove")]
        [Authorize(AuthenticationSchemes =
            JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult RemoveProject(long id)
        {
            var answer = db.RemoveProject(id);
            return new JsonResult(answer);
        }
        [HttpGet("/Projects/Exp")]
        [Authorize(AuthenticationSchemes =
            JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult ExpProject()
        {
            var answer = db.getExcelStream();
            return new JsonResult(answer);
        }
        [HttpPost("/Project/Stages/Add")]
        [Authorize(AuthenticationSchemes =
            JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult AddProjectStage(int idProj, string name,
            string dateS, string dateE)
        {
            var stage = db.CreateStage(idProj,name,dateS,dateE);
            return new JsonResult(stage);
        }


    }
}
