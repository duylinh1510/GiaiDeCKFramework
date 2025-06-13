using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using FlightManagement.Models;

namespace FlightManagement.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
} 