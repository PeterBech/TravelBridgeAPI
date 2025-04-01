using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TravelBridgeAPI.DataHandlers;

namespace TravelBridgeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly HandleLocations _handleLocations;
        private readonly HandleFlightDetails _handleFlightDetails;
        private readonly HandleFlightMinPrice _flightMinPriceHandler;

        public FlightController(HandleLocations handleLocations, HandleFlightDetails handleFlightDetails, HandleFlightMinPrice handleFlightMinPrice)
        {
            _handleLocations = handleLocations;
            _handleFlightDetails = handleFlightDetails;
            _flightMinPriceHandler = handleFlightMinPrice;
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
        public async Task<IActionResult> SearchFlightDetails(string token, string curencyCode)
        {
            var result = await _handleFlightDetails.GetFlightDetails(token, curencyCode);
            if (result == null)
            {
                return NotFound("No flight details found.");
            }
            return Ok(result);
        }

        [HttpGet("FlightMinPrice/")]
        public async Task<IActionResult> SearchMinFlightPrice(
            string from,
            string to,
            string departure,
            string returnFlight,
            string cabinClass,
            string curencyCode)
        {
            if (!IsValidDate(departure) || !IsValidDate(returnFlight))
            {
                return BadRequest("Invalid date format. Please use yyyy-MM-dd.");
            }

            var result = await _flightMinPriceHandler.GetMinFlightPrice(from, to, departure, returnFlight, cabinClass, curencyCode);

            if (result == null)
            {
                return NotFound("No flight details found.");
            }
            return Ok(result);
        }

        private bool IsValidDate(string date)
        {
            return DateOnly.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }
    }
}
