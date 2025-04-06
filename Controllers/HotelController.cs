using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Globalization;
using TravelBridgeAPI.DataHandlers.HotelHandlers;

namespace TravelBridgeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly HandleSearchHotels _handleSearchHotels;
        private readonly HandleSearchDestination _handleSearchDestination;
        public HotelController(HandleSearchHotels handleSearchHotels, HandleSearchDestination handleSearchDestination)
        {
            _handleSearchHotels = handleSearchHotels;
            _handleSearchDestination = handleSearchDestination;
        }

        [HttpGet("SearchHotelDestination/")]
        public async Task<IActionResult> SearchHotelDestination(string location)
        {
            var result = await _handleSearchDestination.GetHotelDestination(location);
            if (result == null)
            {
                return NotFound("No hotel destinations found.");
            }
            return Ok(result);
        }


        [HttpGet("SearchHotels/")]
        public async Task<IActionResult> SearchHotels(
            string dest_id,
            string search_type,
            string arrival,
            string departure,
            string? adults,
            string? children,
            int? room_qty,
            int? page_number,
            int? minPrice,
            int? maxPrice,
            string? units,
            string? tempUnit,
            string? language,
            string? currencyCode,
            string? location)
        {
            // Valider påkrævede felter
            if (string.IsNullOrEmpty(dest_id) || string.IsNullOrEmpty(search_type) || string.IsNullOrEmpty(arrival) || string.IsNullOrEmpty(departure))
            {
                return BadRequest("Missing required parameters: dest_id, search_type, arrival, or departure.");
            }

            // Valider datoformat
            if (!IsValidDate(arrival) || !IsValidDate(departure))
            {
                return BadRequest("Invalid date format. Please use yyyy-MM-dd.");
            }

            DateTime checkIn = DateTime.Parse(arrival);
            DateTime checkOut = DateTime.Parse(departure);

            if (checkOut <= checkIn)
            {
                return BadRequest("Departure date must be after arrival date.");
            }

            try
            {
                var result = await _handleSearchHotels.GetHotel(
                    dest_id,
                    search_type,
                    checkIn.ToString("yyyy-MM-dd"),
                    checkOut.ToString("yyyy-MM-dd"),
                    adults,
                    children,
                    room_qty,
                    page_number,
                    minPrice,
                    maxPrice,
                    units,
                    tempUnit,
                    language,
                    currencyCode,
                    location
                );

                if (result == null)
                {
                    return NotFound("No hotels found.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log fejl (kan tilføjes med Serilog eller anden logger)
                return StatusCode(500, $"An error occurred while processing the request: {ex.Message}");
            }
        }
        private bool IsValidDate(string date)
        {
            return DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }
    }
}
