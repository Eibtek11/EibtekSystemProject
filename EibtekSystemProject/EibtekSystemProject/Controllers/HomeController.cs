using BL;
using EibtekSystemProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EibtekSystemProject.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(UserManager<ApplicationUser> userManager, ILogger<HomeController> logger)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult>  Index()
        {
            
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ViewData["TwoFactorEnabled"] = false;
            }
            else
            {
                ViewData["TwoFactorEnabled"] = user.TwoFactorEnabled;
            }
            return View();
        }
        [Authorize(Roles ="Admin")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
