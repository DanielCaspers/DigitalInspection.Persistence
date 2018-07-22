using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DigitalInspectionNetCore21.Models;
using DigitalInspectionNetCore21.ViewModels.Base;

namespace DigitalInspectionNetCore21.Controllers
{
	// [AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
	public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(new BaseHomeViewModel());
        }

        public IActionResult About()
        {
	        ViewData["Message"] = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

			return View(new BaseAboutViewModel());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
