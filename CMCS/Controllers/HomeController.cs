using CMCS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CMCS.Controllers
{
    public class HomeController : Controller
    {
        //logger instance to log messages, warnings, errors, etc.
        private readonly ILogger<HomeController> _logger;

        //constructor to initialize the logger
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger; //assigns the logger instance
        }

        //action for the home page (Index view)
        public IActionResult Index()
        {
            return View(); //returns the Index view
        }

        //action for the About Us page
        public IActionResult AboutUs()
        {
            return View(); //returns the AboutUs view
        }

        //action for the Privacy page
        public IActionResult Privacy()
        {
            return View(); // returns the Privacy view
        }

        //action for handling errors and displaying error information
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            //creates an ErrorViewModel with the current request ID or a trace identifier
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
