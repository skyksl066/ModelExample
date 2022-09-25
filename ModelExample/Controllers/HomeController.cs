using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ModelExample.Models;

namespace ModelExample.Controllers
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

        public IActionResult Create()
        {
            if(TempData["Message"] != null)
                ViewBag.Message = TempData["Message"];
            return View(new Create());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Create create)
        {
            // 驗證
            if (!ModelState.IsValid)
            {
                string message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return View(create);
            }
            // TODO
            TempData["Message"] = $"請假成功!\n開始日期: {create.StartDate:yyyy/MM/dd}\n結束日期: {create.EndDate:yyyy/MM/dd}\n共計: {create.Days}日 {create.Hour}時";
            return RedirectToAction("Create");
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
