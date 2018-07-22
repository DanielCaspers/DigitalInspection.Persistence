using System;
using Microsoft.AspNetCore.Http;

namespace DigitalInspectionNetCore21.Services.Web
{
	public static class CookieConstants
	{
		public static readonly string CompanyCookieName = "DI_CompanyNumber";
		public static readonly string UserIdCookieName = "DI_UserId";

		public static readonly CookieOptions DefaultOptions = new CookieOptions
		{
			Expires = DateTime.Now.AddYears(10)
		};

	}
}
