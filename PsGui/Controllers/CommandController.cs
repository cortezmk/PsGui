using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PsGui.Projection;

namespace PsGui.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandController : ControllerBase
    {
        [HttpPost]
        public ActionResult<string> Post([FromBody] PostInput input)
        {
            var projector = new BasicProjector();
            var ps = PowerShell.Create();
            ps.AddScript(input.Command);
            var result = ps.Invoke();
            var projectedResult = projector.Project(result);
            return JsonConvert.SerializeObject(projectedResult);
        }
    }
}
