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

		/// <summary>
		/// Get the id of inspection associated with a given work order
		/// </summary>
		/// <param name="id">
		/// Unique identifier of your work order 
		/// (e.g. the unit of sale or invoice ID for services rendered)
		/// </param>
		/// <response code="200">Ok</response>
		/// <response code="404">Not found</response>
		[HttpGet("InspectionId")]
		public ActionResult InspectionIdForOrder(string id)
		{
			var inspectionId = _context.Inspections
				.SingleOrDefault(i => i.WorkOrderId == id)
				?.Id;

			return inspectionId != null ? Json(inspectionId) : (ActionResult)NotFound();
		}

		/// <summary>
		/// Get the inspection report associated with a given work order
		/// </summary>
		/// <remarks>The response is always ordered by severity, from greatest to least</remarks>
		/// <param name="id">
		/// Unique identifier of your work order 
		/// (e.g. the unit of sale or invoice ID for services rendered)
		/// </param>
		/// <param name="grouped">
		/// When true, inspection items are grouped by severity.
		/// When false, inspection items are returned in a flattened representation
		/// </param>
		/// <param name="includeUnknown">
		/// When false, includes inspection items whose conditions are unknown (e.g. Unmarked or unanswered).
		/// This can be useful if you have situations where you find it unnecessary to perform a unit of inspection,
		/// but don't want it communicated to a customer that this step was skipped.
		/// When true, all inspection items are included in the report, regardless of condition.
		/// </param>
		/// <response code="200">Ok</response>
		/// <response code="404">Not found</response>
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

		/// <summary>
		/// Delete the inspection report associated with a given work order
		/// </summary>
		/// <remarks>
		/// This can be useful if you need to
		/// - start an order fresh in your system
		/// - reuse invoice IDs due to rollover mechanisms
		/// </remarks>
		/// <param name="id">
		/// Unique identifier of your work order 
		/// (e.g. the unit of sale or invoice ID for services rendered)
		/// </param>
		/// <response code="204">No content</response>
		/// <response code="500">Unable to delete some part of the inspection</response>
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
