using System.Threading.Tasks;
using DigitalInspectionNetCore21.Models;
using DigitalInspectionNetCore21.Models.DbContexts;
using DigitalInspectionNetCore21.Services.Web;
using DigitalInspectionNetCore21.ViewModels.VehicleHistory;
using Microsoft.AspNetCore.Mvc;

namespace DigitalInspectionNetCore21.Controllers
{
	public class VehicleHistoryController : BaseController
	{
		public VehicleHistoryController(ApplicationDbContext db) : base(db)
		{ 
		}

		[HttpPost]
		[AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public PartialViewResult GetVehicleHistoryDialog(string VIN)
		{
			var task = Task.Run(async () => {
				return await VehicleHistoryService.GetVehicleHistory(CurrentUserClaims, VIN, GetCompanyNumber());
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			return PartialView("../Shared/Dialogs/_VehicleHistoryDialog", new VehicleHistoryViewModel
			{
				VehicleHistory = task.Result.Entity
			});
		}
	}
}
