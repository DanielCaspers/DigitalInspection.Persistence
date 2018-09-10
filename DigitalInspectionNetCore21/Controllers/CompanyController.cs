using System;
using DigitalInspectionNetCore21.Services.Web;
using DigitalInspectionNetCore21.Models;
using DigitalInspectionNetCore21.Models.DbContexts;
using Microsoft.AspNetCore.Mvc;

namespace DigitalInspectionNetCore21.Controllers
{
	public class CompanyController : BaseController
	{
		public CompanyController(ApplicationDbContext db) : base(db) { }

		[HttpPost]
		[AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public ActionResult ChangeCompany(string companyNumber)
		{
			Response.Cookies.Append(CookieConstants.CompanyCookieName, companyNumber, CookieConstants.DefaultOptions);

			return RedirectToAction("Index", "WorkOrders");
		}
	}
}
