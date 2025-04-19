using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TravelBridgeAPI.CustomAttributes;
using TravelBridgeAPI.DataHandlers;

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
            Console.WriteLine($"[INFO] Received SearchLocation request for {location} ({language})");
            var result = await _handleLocations.GetLocationAsync(location, language);
            if (result == null)
            {
                Console.WriteLine($"[WARN] No location data found for {location}");
                return NotFound("No flight locations found.");
            }
            Console.WriteLine($"[SUCCESS] FlightDetails data found for {location}");
            return Ok(result);
        }

        [HttpGet("SearchFlightDetails/")]
        [ApiKey]
        public async Task<IActionResult> SearchFlightDetails(string token, string? currencyCode)
        {
            Console.WriteLine($"[INFO] Received SearchFlightDetails request for {token} ({currencyCode})");
            var result = await _handleFlightDetails.GetFlightDetailsAsync(token, currencyCode);
            if (result == null)
            {
                Console.WriteLine($"[WARN] No FlightDetails data found for {token}");
                return NotFound("No flight details found.");
            }
            Console.WriteLine($"[SUCCESS] FlightDetails found successfully for {token}");
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
            Console.WriteLine($"[INFO] Received SearchMinFlightPrice request for from: {from}, to: {to}, departure: {departure}  ({currencyCode}, {cabinClass}, {currencyCode})");
            // Tjek for manglende obligatoriske felter
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to) || string.IsNullOrWhiteSpace(departure))
            {
                Console.WriteLine($"[WARN] Missing required parameters: from: {from}, to: {to}, departure: {departure}");
                return BadRequest("Missing required parameters: from, to, and departure are required.");
            }

            // Valider datoformat for obligatorisk felt og valgfrit felt
            if (!isValidDate(departure) || (!string.IsNullOrWhiteSpace(returnFlight) && !isValidDate(returnFlight)))
            {
                Console.WriteLine($"[WARN] Invalid date format for departure: {departure} or returnFlight: {returnFlight}");
                return BadRequest("Invalid date format. Please use yyyy-MM-dd.");
            }

            var result = await _flightMinPriceHandler.GetMinFlightPriceAsync(from, to, departure, returnFlight, cabinClass, currencyCode);

            if (result == null)
            {
                Console.WriteLine($"[WARN] No MinFlightPrice data found for from: {from}, to: {to}, departure: {departure}");
                return NotFound("No flight details found.");
            }
            Console.WriteLine($"[SUCCESS] MinFlightPrice found successfully for from: {from}, to: {to}, departure: {departure}");
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
            Console.WriteLine($"[INFO] Received SearchDirectFlights request for departure: {departure}, arrival: {arrival}, date: {date} ({sort}, {cabinClass}, {currency})");
            var result = await _handleSearch.GetDirectFlightAsync(departure, arrival, date, sort, cabinClass, currency);

            if (result == null || result.data?.flightOffers == null || result.data.flightOffers.Length == 0)
            {
                Console.WriteLine($"[WARN] No direct flights found for departure: {departure}, arrival: {arrival}, date: {date}");
                return NotFound("No direct flights found.");
            }
            Console.WriteLine("[SUCCESS] DirectFlights data found for departure: {departure}, arrival: {arrival}, date: {date}");
            return Ok(result);
        }

        private bool isValidDate(string date)
        {

            return DateOnly.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }
    }
}
