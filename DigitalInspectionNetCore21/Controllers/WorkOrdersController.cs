using System.Linq;
using DigitalInspectionNetCore21.Models.DbContexts;
using DigitalInspectionNetCore21.Services.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace DigitalInspectionNetCore21.Controllers
{
	[AllowAnonymous]
	[Route("[controller]/{id}")]
	public class WorkOrdersController : BaseController
	{
		private readonly IHostingEnvironment _env;

		public WorkOrdersController(ApplicationDbContext db, IHostingEnvironment env) : base(db)
		{
			_env = env;
		}

		[HttpGet("InspectionId")]
		public JsonResult InspectionIdForOrder(string id)
		{
			var inspectionId = _context.Inspections
				.SingleOrDefault(i => i.WorkOrderId == id)
				?.Id;

			return Json(inspectionId);
		}

		[HttpGet("Inspection")]
		public ActionResult ReportForOrder(string id, bool grouped = false, bool includeUnknown = false)
		{
			var imageBaseUrl = $"{this.Request.Scheme}://{this.Request.Host}";

			var inspectionItems = InspectionService.GetInspectionItems(_context)
				.SingleOrDefault(i => i.WorkOrderId == id)
				?.InspectionItems;

			if (inspectionItems == null)
			{
				return NotFound();
			}

			return InspectionService.BuildInspectionReport(_context, imageBaseUrl, inspectionItems, grouped, includeUnknown);
		}

		[HttpDelete("Inspection")]
		public ActionResult Delete(string id)
		{
			var inspection = _context.Inspections.SingleOrDefault(i => i.WorkOrderId == id);
			if (inspection == null)
			{
				return NoContent();
			}

			var webRootPath = _env.WebRootPath;
			var wasSuccessful = InspectionService.DeleteInspection(_context, inspection, webRootPath);

			return wasSuccessful ? NoContent() : StatusCode(500);
		}
	}
}
