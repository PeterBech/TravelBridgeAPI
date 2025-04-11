using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Globalization;
using TravelBridgeAPI.CustomAttributes;
using TravelBridgeAPI.DataHandlers.HotelHandlers;
using TravelBridgeAPI.Models.FlightDetails;
using TravelBridgeAPI.Security;

namespace TravelBridgeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]    
    public class HotelController : ControllerBase
    {
        private readonly HandleSearchHotels _handleSearchHotels;
        private readonly HandleSearchDestination _handleSearchDestination;
        private readonly HandleReviewScores _handleReviewScores;
        private readonly HandleRoomAvailability _handleRoomAvailability;
        private readonly IApiKeyValidation _apiKeyValidation;
        public HotelController(
            HandleSearchHotels handleSearchHotels, 
            HandleSearchDestination handleSearchDestination, 
            HandleReviewScores handleReviewScores, 
            HandleRoomAvailability handleRoomAvailability,
            IApiKeyValidation apiKeyValidation)
        {
            _handleSearchHotels = handleSearchHotels;
            _handleSearchDestination = handleSearchDestination;
            _handleReviewScores = handleReviewScores;
            _handleRoomAvailability = handleRoomAvailability;
            _apiKeyValidation = apiKeyValidation;
        }

        [HttpGet("SearchHotelDestination/")]
        [ApiKey]
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
        [ApiKey]
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

        [HttpGet("SearchReviewScore/")]
        [ApiKey]
        public async Task<IActionResult> SearchReviewScore(int hotel_id, string? language)
        {
            if (hotel_id <= 0)
            {
                return BadRequest("Invalid hotel ID.");
            }
            try
            {
                var result = await _handleReviewScores.GetHotelReviewScores(hotel_id, language);
                if (result == null)
                {
                    return NotFound("No review scores found for the specified hotel.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log fejl (kan tilføjes med Serilog eller anden logger)
                return StatusCode(500, $"An error occurred while processing the request: {ex.Message}");
            }
        }

        [HttpGet("SearchRoomAvailability/")]
        [ApiKey]
        public async Task<IActionResult> SearchRoomAvailability(
            int hotel_id, 
            string? min_date,
            string? max_date,
            int? rooms,
            int? adults,
            string? currencyCode,
            string? location)
        {
            if (hotel_id <= 0)
            {
                return BadRequest("Invalid hotel ID.");
            }
            // Valider datoformat
            if (!string.IsNullOrEmpty(min_date) && !IsValidDate(min_date))
            {
                return BadRequest("Invalid min_date format. Please use yyyy-MM-dd.");
            }
            if (!string.IsNullOrEmpty(max_date) && !IsValidDate(max_date))
            {
                return BadRequest("Invalid max_date format. Please use yyyy-MM-dd.");
            }
            try
            {
                var result = await _handleRoomAvailability.GetRoomAvailability(
                    hotel_id,
                    min_date,
                    max_date,
                    rooms,
                    adults,
                    currencyCode,
                    location
                );
                if (result == null)
                {
                    return NotFound("No room availability found for the specified hotel.");
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
