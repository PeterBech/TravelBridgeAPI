using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TravelBridgeAPI.CustomAttributes;
using TravelBridgeAPI.DataHandlers.FlightHandlers;

namespace TravelBridgeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class FlightController : ControllerBase
    {
        private readonly HandleLocations _handleLocations;
        private readonly HandleFlightDetails _handleFlightDetails;
        private readonly HandleFlightMinPrice _flightMinPriceHandler;
        private readonly HandleSearch _handleSearch;

        public FlightController(HandleLocations handleLocations, HandleFlightDetails handleFlightDetails, HandleFlightMinPrice handleFlightMinPrice, HandleSearch handleSearch)
        {
            _handleLocations = handleLocations;
            _handleFlightDetails = handleFlightDetails;
            _flightMinPriceHandler = handleFlightMinPrice;
            _handleSearch = handleSearch;
        }

        [HttpGet("SearchLocations/")]
        [ApiKey]
        public async Task<IActionResult> SearchLocation(string location, string? language)
        {
            if (language == null)
                language = "en-gb";
            
            var result = await _handleLocations.GetLocationAsync(location, language);
            if (result == null)
            {
                return NotFound("No flight locations found.");
            }
            return Ok(result);
        }

        [HttpGet("SearchFlightDetails/")]
        [ApiKey]
        public async Task<IActionResult> SearchFlightDetails(string token, string? currencyCode)
        {
            
            var result = await _handleFlightDetails.GetFlightDetailsAsync(token, currencyCode);
            if (result == null)
            {
                return NotFound("No flight details found.");
            }
            return Ok(result);
        }

        [HttpGet("FlightMinPrice/")]
        [ApiKey]
        public async Task<IActionResult> SearchMinFlightPrice(
            string from,
            string to,
            string departure,
            string? returnFlight,
            string? cabinClass,
            string? currencyCode)
        {
            // Tjek for manglende obligatoriske felter
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to) || string.IsNullOrWhiteSpace(departure))
            {
                return BadRequest("Missing required parameters: from, to, and departure are required.");
            }

            // Valider datoformat for obligatorisk felt og valgfrit felt
            if (!isValidDate(departure) || (!string.IsNullOrWhiteSpace(returnFlight) && !isValidDate(returnFlight)))
            {
                return BadRequest("Invalid date format. Please use yyyy-MM-dd.");
            }

            var result = await _flightMinPriceHandler.GetMinFlightPriceAsync(from, to, departure, returnFlight, cabinClass, currencyCode);

            if (result == null)
            {
                return NotFound("No flight details found.");
            }
            return Ok(result);
        }

        [HttpGet("SearchDirectFlights/")]
        [ApiKey]
        public async Task<IActionResult> SearchDirectFlights(
            string departure,
            string arrival,
            string date,
            string? sort = null,
            string? cabinClass = null,
            string? currency = null)
        {
            var result = await _handleSearch.GetDirectFlightAsync(departure, arrival, date, sort, cabinClass, currency);

            if (result == null || result.data?.flightOffers == null || result.data.flightOffers.Length == 0)
            {
                return NotFound("No direct flights found.");
            }
            return Ok(result);
        }

        private bool isValidDate(string date)
        {

            return DateOnly.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }
    }
}
