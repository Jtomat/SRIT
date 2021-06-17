using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnSleepingEyeServer.Support;

namespace UnSleepingEyeServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupportFunctionsController : ControllerBase
    {
        [HttpPost("/Analyze/Keywords")]
        public async Task<IActionResult> GetKeyWords()
        {
            string rawValue = "";
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                rawValue = await reader.ReadToEndAsync();
            }
            var file = Convert.FromBase64String(rawValue.Substring(rawValue.IndexOf(",")+1));
            var strS = FileFormat.GetContentFrom(file, FileFormat.GetMimeFromBytes(file)[1]);
            var stage = (new DataAnalyzer()).GetKeyWordsForText(strS);
            return new JsonResult(stage);
        }
    }
}
