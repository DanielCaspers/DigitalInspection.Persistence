using System.Collections.Generic;
using System.Security.Claims;
using DigitalInspectionNetCore21.Models.DbContexts;
using DigitalInspectionNetCore21.Services.Web;
using Microsoft.AspNetCore.Mvc;

namespace DigitalInspectionNetCore21.Controllers
{
	public abstract class BaseController : Controller
	{
		protected string ResourceName;

		protected ApplicationDbContext _context;

		protected BaseController(ApplicationDbContext db)
		{
			_context = db;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			_context.Dispose();
		}

		// FIXME DJC NEEDS AUTH
		//protected IEnumerable<Claim> CurrentUserClaims => Request.GetOwinContext().Authentication.User.Claims;
		protected IEnumerable<Claim> CurrentUserClaims => new List<Claim>(){};

		// TODO: Determine if needed
		//protected override void OnException(ExceptionContext filterContext)
		//{
		//	filterContext.ExceptionHandled = true;
		//	var exception = filterContext.Exception;

		//	var info = new HandleErrorInfo(
		//		exception,
		//		filterContext.RouteData.Values["controller"].ToString(),
		//		filterContext.RouteData.Values["action"].ToString()
		//	);

		//	filterContext.Result = View(
		//		"~/Views/Shared/Error.cshtml",
		//		new BaseErrorModel() {
		//			Toast = ToastService.UnknownErrorOccurred(exception, info),
		//			Error = info,
		//			StackTrace = new StackTrace(exception)
		//		}
		//	);
		//}

		// Retreives company number from cookie
		protected string GetCompanyNumber()
		{
			Request.Cookies.TryGetValue(CookieConstants.CompanyCookieName, out var cookieContents);
			return cookieContents;
		}
	}
}
