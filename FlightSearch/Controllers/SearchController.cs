using FlightSearch.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FlightSearch.Controllers
{
    [Route("api/[controller]")]

    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IFlightSearchService _service;

        public SearchController(IFlightSearchService service)
        {
            _service = service;
        }
        [HttpGet]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchFlight([Required] string OriginCode, [Required] string DestinationCode, [Required] DateTime departureDate, string sortType)
        {
            if (string.IsNullOrEmpty(OriginCode) || OriginCode.Length != 3 || string.IsNullOrEmpty(DestinationCode) || DestinationCode.Length != 3)
            {
                return BadRequest(new ProblemDetails
                {
                    Type = "https://httpstatuses.com/400",
                    Title = "Bad Request",
                    Detail = "Invalid Airport Codes"
                });
            }
            departureDate = departureDate.Date;
            if (departureDate < DateTime.Today)
            {
                return BadRequest(new ProblemDetails
                {
                    Type = "https://httpstatuses.com/400",
                    Title = "Bad Request",
                    Detail = "Date should be of today and future"
                });
            }
            if ((!string.IsNullOrEmpty(sortType))&&!( sortType.Trim().ToUpper()=="COST" || sortType.Trim().ToUpper() == "DEPARTURE_DATETIME"))
            {
                return BadRequest(new ProblemDetails
                {
                    Type = "https://httpstatuses.com/400",
                    Title = "Bad Request",
                    Detail = "Invalid SortType. Valid Values are COST and DEPARTURE_DATETIME"
                });
            }

            var result = await _service.FindFlight(OriginCode, DestinationCode, departureDate, sortType);
            return Ok(result);
        }
    }
}
