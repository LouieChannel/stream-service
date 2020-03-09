using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ascalon.StreamService.DumperService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Ascalon.StreamService.Controllers
{
    [ApiController]
    [Route("control")]
    public class ControlController : Controller
    {
        private readonly ILogger<ControlController> _logger;
        private readonly IDumperService _dumperService;

        private Thread getData;
        private Thread processData;
        private Thread sendData;

        public ControlController(ILogger<ControlController> logger, IDumperService dumperService)
        {
            _logger = logger;
            _dumperService = dumperService;

            getData = new Thread(new ThreadStart(_dumperService.GetDataFromDumper));
            processData = new Thread(new ThreadStart(_dumperService.ProcessDataFromDumper));
            sendData = new Thread(new ThreadStart(_dumperService.SendDataToDumperService));
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

                getData.Start();

                processData.Start();

                sendData.Start();

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