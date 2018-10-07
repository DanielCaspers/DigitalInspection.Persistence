using System.Linq;
using System;
using System.Collections.Generic;
using AutoMapper;
using DigitalInspectionNetCore21.Models.Orders;
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

		[HttpGet("")]
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

		[HttpPut("Condition")]
		public ActionResult SetCondition(Guid id, RecommendedServiceSeverity inspectionItemCondition)
		{
			var inspectionItemInDb = _context.InspectionItems.SingleOrDefault(item => item.Id == id);

			if (inspectionItemInDb == null)
			{
				return NotFound();
			}

			return InspectionService.UpdateInspectionItemCondition(_context, inspectionItemInDb, inspectionItemCondition) ?
				NoContent() : 
				StatusCode(500);
		}

		[HttpPut("CannedResponses")]
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

		[HttpPut("CustomerConcern")]
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

		[HttpPut("Notes")]
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

		[HttpPut("Measurements")]
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
