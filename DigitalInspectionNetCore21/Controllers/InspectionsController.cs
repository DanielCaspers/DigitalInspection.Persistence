using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using DigitalInspectionNetCore21.Models.DbContexts;
using DigitalInspectionNetCore21.Models.Inspections;
using DigitalInspectionNetCore21.Services.Core;
using DigitalInspectionNetCore21.Services.Core.Interfaces;
using DigitalInspectionNetCore21.ViewModels.Inspections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

		[AllowAnonymous]
		[HttpGet("{id}/Report")]
		public JsonResult Report(Guid id, bool grouped = false, bool includeUnknown = false)
		{
			var imageBaseUrl = $"{this.Request.Scheme}://{this.Request.Host}";

			var inspectionItems = InspectionService.GetInspectionItems(_context)
				.Single(i => i.Id == id)
				.InspectionItems;

			return InspectionService.BuildInspectionReport(_context, imageBaseUrl, inspectionItems, grouped, includeUnknown);
		}

		[HttpGet("{id}/WorkOrderId")]
		public ActionResult WorkOrderId(Guid id)
		{
			var workOrderId = _context.Inspections
				.SingleOrDefault(i => i.Id == id)
				?.WorkOrderId;

			return workOrderId != null ? Json(workOrderId) : (ActionResult)NotFound();
		}

		[HttpPost]
		// [AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public PartialViewResult GetViewInspectionPhotosDialog(Guid inspectionItemId, Guid checklistItemId, Guid checklistId, Guid? tagId, string workOrderId)
		{
			var checklistItem = _context.ChecklistItems.SingleOrDefault(ci => ci.Id == checklistItemId);
			var inspectionItem = _context.InspectionItems
				.Include(ii => ii.InspectionImages)
				.Single(item => item.Id == inspectionItemId);

			IList<InspectionImage> images = inspectionItem.InspectionImages
				.Select((image) =>
				{
					image.Title = Path.Combine($"/Uploads/{IMAGE_DIRECTORY}/{workOrderId}/{inspectionItemId.ToString()}/", image.Title);
					return image;
				})
				.OrderBy(image => image.CreatedDate)
				.ToList();

			return PartialView("_ViewInspectionPhotosDialog", new ViewInspectionPhotosViewModel
			{
				ChecklistItem = checklistItem,
				Images = images,
				ChecklistId = checklistId,
				TagId = tagId,
				WorkOrderId = workOrderId
			});
		}
	}
}
