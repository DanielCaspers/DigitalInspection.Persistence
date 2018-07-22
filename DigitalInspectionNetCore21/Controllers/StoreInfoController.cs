using System.Threading.Tasks;
using DigitalInspectionNetCore21.Models.DbContexts;
using DigitalInspectionNetCore21.Services.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalInspectionNetCore21.Controllers
{
	public class StoreInfoController : BaseController
	{
		public StoreInfoController(ApplicationDbContext db) : base(db)
		{
			ResourceName = "StoreInfo";
		}

		[AllowAnonymous]
		public JsonResult Json(string companyNumber)
		{
			var task = Task.Run(async () => {
				return await StoreInfoService.GetStoreInfo(companyNumber);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			return Json(task.Result.Entity);
		}
	}
}
