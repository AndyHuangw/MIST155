﻿using Microsoft.AspNetCore.Mvc;
using MIST155.Models;
using MIST155.Models.DTO;
using System.Diagnostics;

namespace MIST155.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
       


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Rigister()
        {
            return View();
        }
        public IActionResult Jsontest()
        {
            return View();
        }
        public IActionResult First()
        {
            return View();
        }
        public IActionResult Cors()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Spot() 
        {
            return View();
        }
        public IActionResult AutoComplete()
        {
            return View();
        }
        public IActionResult Image()
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
