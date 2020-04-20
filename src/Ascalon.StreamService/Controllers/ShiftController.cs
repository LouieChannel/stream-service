using Ascalon.StreamService.DumperService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Ascalon.StreamService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShiftController : ControllerBase
    {
        private readonly ILogger<ShiftController> _logger;
        private readonly IDumperService _dumperService;

        public ShiftController(ILogger<ShiftController> logger, IDumperService dumperService)
        {
            _logger = logger;
            _dumperService = dumperService;
        }

        [HttpGet("start/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult Start([FromRoute]int id)
        {
            try
            {
                _dumperService.InitShift(id);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when trying to start shift: Number driver id: {id}.");
                return StatusCode(500);
            }
        }

        [HttpGet("stop/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult Stop([FromRoute]int id)
        {
            try
            {
                _dumperService.EndShift(id);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when trying to start shift. Number driver id: {id}.");
                return StatusCode(500);
            }
        }
    }
}