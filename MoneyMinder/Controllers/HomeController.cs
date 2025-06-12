using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MoneyMinder.Models;

namespace MoneyMinder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MoneyMinder.Services.ILogService _logService;

        public HomeController(ILogger<HomeController> logger, MoneyMinder.Services.ILogService logService)
        {
            _logger = logger;
            _logService = logService;
        }

        public IActionResult Index()
        {
            _logService.LogInfo("Index action executed");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
