using Microsoft.AspNetCore.Mvc;
using TravelBridgAPI.DataHandler;
using TravelBridgAPI.Models;

namespace TravelBridgAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly HandleLocations _handleLocations;

        public FlightController(HandleLocations handleLocations)
        {
            _handleLocations = handleLocations;
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
    }
}
