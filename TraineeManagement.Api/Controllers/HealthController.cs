using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TraineeManagement.Api.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [AllowAnonymous]
    public class HealthController : ControllerBase
    {   
        private readonly HealthCheckService _service ;

        public HealthController( HealthCheckService service )
        {
            _service = service;
        }

        [HttpGet("live")]
        public IActionResult GetLiveness()
        {
            return Ok(new
            {
                status = "running",
                application = "Trainee Management API",
                timestamp = DateTime.Now
            }) ;
        }

        [HttpGet("ready")]
        public async Task<IActionResult> GetReadiness()
        {
            var report = await _service.CheckHealthAsync();

            var response = new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(x => new
                {
                    service = x.Key,
                    status = x.Value.Status,
                    description = x.Value.Description
                })
            };

            if( report.Status == HealthStatus.Healthy)
            {
                return Ok(response);
            }

            return StatusCode(503, response);
        }
    }
}