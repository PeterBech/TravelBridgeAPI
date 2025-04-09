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
        private readonly HandleReviewScores _handleReviewScores;
        /// <summary>
        /// Initializes a new instance of the HotelController class with the necessary dependencies for hotel search, destination lookup, and review score retrieval.
        /// </summary>
        public HotelController(HandleSearchHotels handleSearchHotels, HandleSearchDestination handleSearchDestination, HandleReviewScores handleReviewScores)
        {
            _handleSearchHotels = handleSearchHotels;
            _handleSearchDestination = handleSearchDestination;
            _handleReviewScores = handleReviewScores;
        }

        /// <summary>
        /// Asynchronously retrieves hotel destination data based on the provided location.
        /// </summary>
        /// <param name="location">The location query used to search for hotel destinations.</param>
        /// <returns>
        /// An IActionResult with:
        /// <list type="bullet">
        /// <item>
        /// <description>200 OK containing the destination data if found.</description>
        /// </item>
        /// <item>
        /// <description>404 Not Found when no destinations are found.</description>
        /// </item>
        /// </list>
        /// </returns>
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


        /// <summary>
        /// Searches for hotels matching the provided criteria.
        /// </summary>
        /// <remarks>
        /// Validates required parameters (dest_id, search_type, arrival, departure) and ensures that the arrival and departure dates are provided in the "yyyy-MM-dd" format with the departure date occurring after the arrival date.
        /// On successful validation, the method retrieves matching hotels using an internal service and returns a 200 OK response with the results. If no hotels are found, or if validation fails, it returns a 404 Not Found or 400 Bad Request response respectively. In the event of an exception, a 500 Internal Server Error response is returned.
        /// </remarks>
        /// <param name="arrival">The check-in date in "yyyy-MM-dd" format.</param>
        /// <param name="departure">The check-out date in "yyyy-MM-dd" format.</param>
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

        /// <summary>
        /// Retrieves hotel review scores for the specified hotel using its unique identifier and optional language preference.
        /// </summary>
        /// <param name="hotel_id">The unique identifier of the hotel. Must be a value greater than 0.</param>
        /// <param name="language">An optional language code for retrieving localized review scores.</param>
        /// <returns>
        /// An IActionResult containing:
        /// - a 200 OK response with the review scores if found,
        /// - a 400 Bad Request if the hotel ID is invalid,
        /// - a 404 Not Found if no review scores are available,
        /// - or a 500 Internal Server Error if an exception occurs.
        /// </returns>
        [HttpGet("SearchReviewScore/")]
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
        /// <summary>
        /// Checks whether the input string represents a valid date in the "yyyy-MM-dd" format.
        /// </summary>
        /// <param name="date">The date string to validate, expected in "yyyy-MM-dd" format.</param>
        /// <returns>True if the date string is valid; otherwise, false.</returns>
        private bool IsValidDate(string date)
        {
            return DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }
    }
}
