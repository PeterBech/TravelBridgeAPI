using Microsoft.AspNetCore.Mvc;
using TravelBridgAPI.DataHandler;

namespace TravelBridgAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly HandleLocations _handleLocations;
        private readonly HandleFlightDetails _handleFlightDetails;

        public FlightController(HandleLocations handleLocations, HandleFlightDetails handleFlightDetails)
        {
            _handleLocations = handleLocations;
            _handleFlightDetails = handleFlightDetails;
        }

        [HttpGet("SearchLocations/")]
        public async Task<IActionResult> SearchLocation(string location)
        {
            var result = await _handleLocations.GetLocationAsync(location);
            if (result == null)
            {
                return NotFound("No flight locations found.");
            }
            return Ok(result);
        }

        [HttpGet("SearchFlightDetails/")]
        public async Task<IActionResult> SearchFlightDetails(string token, string cc)
        {
            var result = await _handleFlightDetails.GetFlightDetailsAsync(token, cc);
            if (result == null)
            {
                return NotFound("No flight details found.");
            }
            return Ok(result);
        }
    }
}
