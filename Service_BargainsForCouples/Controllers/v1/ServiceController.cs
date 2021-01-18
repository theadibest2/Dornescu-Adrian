using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Service_BargainsForCouples.Common;
using Service_BargainsForCouples.Data.Entities.v1;
using Service_BargainsForCouples.Provider.Service.v1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Service_BargainsForCouples.Controllers.v1
{
    [ApiController]
    [Route("[controller]")]
    public class ServiceController : ControllerBase
    {
        private static bool isFirstRequest = true;

        private readonly ILogger<ServiceController> _logger;

        private readonly IProvider _provider;

        private readonly IOptions<ProviderConfiguration> _cfg;

        public ServiceController(ILogger<ServiceController> logger, IProvider provider, IOptions<ProviderConfiguration> cfg)
        {
            _logger = logger;
            _provider = provider;
            _cfg = cfg;
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        [Route("CalculateHotelsBoardTypes")]
        public async Task<ActionResult<IEnumerable<HotelEntity>>> CalculateHotelsBoardTypes(int destinationId, int noNights)
        {
            var stopwatch = Stopwatch.StartNew();
            CancellationTokenSource s_cts = new CancellationTokenSource();
            if (destinationId <= 0)
                return BadRequest("Invalid destination Id");
            if (noNights <= 0)
                return BadRequest("Invalid number of nights");

            IEnumerable<HotelEntity> hotels = new List<HotelEntity>();
            try
            {
                s_cts.CancelAfter(isFirstRequest ? 500 : 950); //The first request is performed slower because the services have to load and we have to  prevent exceding the 1 sec 
                isFirstRequest = false;
                string fullUrl = string.Format("{0}{1}?code={2}&destinationId={3}&nights={4}", _cfg.Value.UrlApi, ConstantsConfiguration.PROVIDER_ENDPOINT_FIND_BARGAIN, _cfg.Value.Token, destinationId, noNights);
                hotels = await _provider.ProcessUrlAsync(fullUrl, 2, s_cts.Token);
            }
            catch (Exception e)
            {
                if (e is TaskCanceledException || e is OperationCanceledException)
                {
                    _logger.LogInformation("Time expired - return empty list");
                    return Ok(new List<HotelEntity>());
                }
                else
                {
                    _logger.LogError(e.Message, e.StackTrace);
                    return BadRequest("Internal server error :" + e.Message);
                }
            }//catch            

            stopwatch.Stop();
            var measureTime = stopwatch.Elapsed;
            return Ok(hotels);
        }//CalculateHotelsBoardTypes

    }
}
