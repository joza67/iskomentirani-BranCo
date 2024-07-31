using Microsoft.AspNetCore.Mvc;

namespace LoRinoBackend.Controllers
{
    // GWController is a basic controller for handling requests related to "GW" views
    public class GWController : Controller
    {
        // Action method for displaying the default view of the GWController
        public IActionResult Index()
        {
            // Returns the view associated with the Index action
            return View();
        }
    }
}
