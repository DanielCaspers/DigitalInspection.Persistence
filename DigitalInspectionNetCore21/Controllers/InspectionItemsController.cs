using System.Linq;
using System;
using System.Collections.Generic;
using AutoMapper;
using DigitalInspectionNetCore21.Models.DbContexts;
using DigitalInspectionNetCore21.Models.Web.Inspections;
using DigitalInspectionNetCore21.Services.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalInspectionNetCore21.Controllers
{
	[Route("[controller]/{id}")]
	// [AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
	public class InspectionItemsController : BaseController
	{
		private static readonly string IMAGE_DIRECTORY = "Inspections";

		public InspectionItemsController(ApplicationDbContext db) : base(db)
		{
		}

		/// <summary>
		/// Get an inspection item
		/// </summary>
		/// <param name="id">Id of the inspection item</param>
		/// <response code="200">Ok</response>
		/// <response code="404">Not found</response>
		[HttpGet("")]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		public ActionResult<InspectionItemResponse> GetById(Guid id)
		{
			var inspectionItem = _context.InspectionItems.SingleOrDefault(ii => ii.Id == id);

			if (inspectionItem == null)
			{
				return NotFound();
			}
			else
			{
				var checklistItemResponse = Mapper.Map<InspectionItemResponse>(inspectionItem);
				return Json(checklistItemResponse);
			}
		}

		/// <summary>
		/// Mark the condition of an inspection item
		/// </summary>
		/// <param name="id">Id of the inspection item</param>
		/// <param name="condition">Condition of the item being inspected</param>
		/// <response code="204">No content</response>
		/// <response code="404">Inspection item not found</response>
		/// <response code="500">Unable to update condition</response>
		[HttpPut("Condition")]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public ActionResult SetCondition(Guid id, Condition condition)
		{
			var inspectionItemInDb = _context.InspectionItems.SingleOrDefault(item => item.Id == id);

			if (inspectionItemInDb == null)
			{
				return NotFound();
			}

			return InspectionService.UpdateInspectionItemCondition(_context, inspectionItemInDb, (Models.Inspections.InspectionItemCondition) condition) ?
				NoContent() : 
				StatusCode(500);
		}

		/// <summary>
		/// Mark the applicable canned responses for an inspection item
		/// </summary>
		/// <param name="id">Id of the inspection item</param>
		/// <param name="selectedCannedResponseIds">Ids of the canned responses which apply to this inspection item</param>
		/// <response code="204">No content</response>
		/// <response code="404">Inspection item not found</response>
		/// <response code="500">Unable to set the applicable canned responses</response>
		[HttpPut("CannedResponses")]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public ActionResult SetCannedResponses(Guid id, [FromBody] IEnumerable<Guid> selectedCannedResponseIds)
		{
			var inspectionItemInDb = _context.InspectionItems
				.Include(ii => ii.InspectionItemCannedResponses)
					.ThenInclude(iicr => iicr.CannedResponse)
				.SingleOrDefault(item => item.Id == id);

			if (inspectionItemInDb == null)
			{
				return NotFound();
			}

			return InspectionService.UpdateInspectionItemCannedResponses(_context, inspectionItemInDb, selectedCannedResponseIds.ToList()) ? 
				NoContent() :
				StatusCode(500);
		}

		/// <summary>
		/// Mark whether or not this inspection item addresses a customer's concern, or reason, for requesting the inspection
		/// </summary>
		/// <param name="id">Id of the inspection item</param>
		/// <param name="isCustomerConcern">Boolean indicating whether or not this is of concern to the customer</param>
		/// <response code="204">No content</response>
		/// <response code="404">Inspection item not found</response>
		/// <response code="500">Unable to update the customer's concern</response>
		[HttpPut("CustomerConcern")]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public ActionResult SetCustomerConcern(Guid id, [FromBody] bool isCustomerConcern)
		{
			var inspectionItemInDb = _context.InspectionItems.SingleOrDefault(item => item.Id == id);

			if (inspectionItemInDb == null)
			{
				return NotFound();
			}

			return InspectionService.UpdateIsCustomerConcern(_context, inspectionItemInDb, isCustomerConcern) ?
				NoContent() :
				StatusCode(500);
		}

		/// <summary>
		/// Add or update custom notes to be shared with the customer regarding any pertinent details
		/// </summary>
		/// <param name="id">Id of the inspection item</param>
		/// <param name="notes">Notes capturing inspection item observations</param>
		/// <response code="204">No content</response>
		/// <response code="404">Inspection item not found</response>
		/// <response code="500">Unable to update the inspection item notes</response>
		[HttpPut("Notes")]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public ActionResult SetItemNotes(Guid id, [FromBody] string notes)
		{
			var inspectionItemInDb = _context.InspectionItems.SingleOrDefault(item => item.Id == id);

			if (inspectionItemInDb == null)
			{
				return NotFound();
			}

			return InspectionService.UpdateInspectionItemNote(_context, inspectionItemInDb, notes) ?
				NoContent() :
				StatusCode(500);
		}

		/// <summary>
		/// Update measurements to be collected with their values
		/// </summary>
		/// <param name="id">Id of the inspection item</param>
		/// <param name="measurementUpdates">Key value pairs of ids of the measurements being collected, and their values</param>
		/// <response code="204">No content</response>
		/// <response code="404">Inspection item not found</response>
		/// <response code="500">Unable to update the inspection item's measurement values</response>
		[HttpPut("Measurements")]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public ActionResult SetMeasurements(Guid id, [FromBody] IEnumerable<UpdateInspectionMeasurementRequest> measurementUpdates)
		{
			var inspectionItemInDb = _context.InspectionItems.SingleOrDefault(item => item.Id == id);

			if (inspectionItemInDb == null)
			{
				return NotFound();
			}

			return InspectionService.UpdateInspectionItemMeasurements(
				_context,
				measurementUpdates) ?

				NoContent() :
				StatusCode(500);
		}
	}
}
