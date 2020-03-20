using Ascalon.StreamService.DumperService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Ascalon.StreamService.Controllers
{
    [ApiController]
    [Route("control")]
    public class ControlController : Controller
    {
        private readonly ILogger<ControlController> _logger;
        private readonly IDumperService _dumperService;

        public ControlController(ILogger<ControlController> logger, IDumperService dumperService)
        {
            _logger = logger;
            _dumperService = dumperService;
        }

        [HttpGet]
        [Route("run")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult Run()
        {
            try
            {
                _dumperService.Stop = false;

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when trying to start sevice.");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("stop")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult Stop()
        {
            try
            {
                _dumperService.Stop = true;

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when trying to start sevice.");
                return StatusCode(500);
            }
        }
    }
}