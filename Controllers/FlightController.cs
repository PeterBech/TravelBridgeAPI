using Microsoft.AspNetCore.Mvc;
using TravelBridgAPI.DataHandler;

namespace TravelBridgAPI.Controllers
{
    public class FlightController : Controller
    {
        [HttpGet:($"/SearchFlights/{start}&{stop}")]
        public async Task<IActionResult> SearchFlights(string start, string stop)
        {
            return null;
        }
        [HttpGet:($"/SearchLocations/{location}")]
        public async Task<IActionResult> SearchLocation(string location)
        {
            HandleLocations locations = new HandleLocations();
            return locations.GetLocation(location);
        }
    }
}
