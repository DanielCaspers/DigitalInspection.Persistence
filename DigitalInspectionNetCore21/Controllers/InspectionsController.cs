using System.Linq;
using System;
using DigitalInspectionNetCore21.Models.DbContexts;
using DigitalInspectionNetCore21.Services.Core;
using DigitalInspectionNetCore21.Services.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalInspectionNetCore21.Controllers
{
	[Route("[controller]")]
	public class InspectionsController : BaseController
	{
		private readonly ITagRepository _tagRepository;
		private static readonly string IMAGE_DIRECTORY = "Inspections";

		public InspectionsController(ApplicationDbContext db, ITagRepository tagRepository) : base(db)
		{
			_tagRepository = tagRepository;
		}

		/// <summary>
		/// Get the inspection report
		/// </summary>
		/// <remarks>The response is always ordered by severity, from greatest to least</remarks>
		/// <param name="id">Id of the inspection</param>
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
		[AllowAnonymous]
		[HttpGet("{id}/Report")]
		public JsonResult Report(Guid id, bool grouped = false, bool includeUnknown = false)
		{
			var imageBaseUrl = $"{this.Request.Scheme}://{this.Request.Host}";

			// TODO: DJC this looks supiciously wrong... Investigate
			var inspectionItems = InspectionService.GetInspectionItems(_context)
				.Single(i => i.Id == id)
				.InspectionItems;

			return InspectionService.BuildInspectionReport(_context, imageBaseUrl, inspectionItems, grouped, includeUnknown);
		}

		/// <summary>
		/// Get the id of work order associated with a given inspection 
		/// </summary>
		/// <param name="id">Id of the inspection</param>
		/// <response code="200">Ok</response>
		/// <response code="404">Not found</response>
		[HttpGet("{id}/WorkOrderId")]
		public ActionResult WorkOrderId(Guid id)
		{
			var workOrderId = _context.Inspections
				.SingleOrDefault(i => i.Id == id)
				?.WorkOrderId;

			return workOrderId != null ? Json(workOrderId) : (ActionResult)NotFound();
		}
	}
}
