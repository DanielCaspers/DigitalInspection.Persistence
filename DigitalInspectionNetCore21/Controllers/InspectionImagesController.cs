using System;
using System.Linq;
using DigitalInspectionNetCore21.Models.DbContexts;
using DigitalInspectionNetCore21.Models.Inspections;
using DigitalInspectionNetCore21.Services.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalInspectionNetCore21.Controllers
{
	[Route("InspectionItems/{inspectionItemId}/[controller]")]
	// [AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
	public class InspectionImagesController : BaseController
	{
		public InspectionImagesController(ApplicationDbContext db) : base(db) { }

		private static readonly string ImageDirectory = "Inspections";

		[HttpPost("")]
		public ActionResult Upload(Guid inspectionItemId, IFormFile image)
		{
			var inspectionItemInDb = _context.InspectionItems
				.Include(ii => ii.Inspection)
				.SingleOrDefault(item => item.Id == inspectionItemId);

			if (inspectionItemInDb == null)
			{
				return NotFound();
			}

			var workOrderId = inspectionItemInDb.Inspection.WorkOrderId;

			// New guid is used as a random prefix to the filename to ensure uniqueness
			Image imageDto = ImageService.SaveImage(image, new[] { ImageDirectory, workOrderId, inspectionItemId.ToString() }, Guid.NewGuid().ToString(), false);

			return InspectionService.AddInspectionItemImage(_context, inspectionItemInDb, imageDto) ?
				NoContent() :
				StatusCode(500);
		}

		[HttpDelete("{imageId}")]
		public ActionResult Delete(Guid inspectionItemId, Guid imageId)
		{
			var image = _context.InspectionImages.SingleOrDefault(inspectionImage => inspectionImage.Id == imageId);

			if (image == null)
			{
				return NotFound();
			}

			var inspectionItemInDb = _context.InspectionItems.SingleOrDefault(inspectionItem => inspectionItem.Id == inspectionItemId);

			if (inspectionItemInDb == null)
			{
				return NotFound();
			}

			ImageService.DeleteImage(image);

			return InspectionService.DeleteInspectionItemImage(_context, image) ?
				NoContent() :
				StatusCode(500);
		}

		[HttpPut("{imageId}/Visibility")]
		public ActionResult SetCustomerVisibility(Guid inspectionItemId, Guid imageId, [FromBody] bool isVisibleToCustomer)
		{
			var inspectionItemInDb = _context.InspectionItems
					.Include(item => item.InspectionImages)
				.SingleOrDefault(item => item.Id == inspectionItemId);
			if (inspectionItemInDb == null)
			{
				return NotFound();
			}

			var inspectionImage = inspectionItemInDb.InspectionImages.SingleOrDefault(ii => ii.Id == imageId);
			if (inspectionImage == null)
			{
				return NotFound();
			}

			return InspectionService.UpdateInspectionImageVisibility(
				_context,
				inspectionImage,
				isVisibleToCustomer) ?

				NoContent() :
				StatusCode(500);
		}
	}
}
