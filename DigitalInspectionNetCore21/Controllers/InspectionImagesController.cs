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

		/// <summary>
		/// Add an image to an inspection item
		/// </summary>
		/// <param name="inspectionItemId">Id of the inspection item to add the image to</param>
		/// <param name="image">The image to attach</param>
		/// <response code="204">No content</response>
		/// <response code="404">Inspection item not found</response>
		/// <response code="500">Unable to save image</response>
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

		/// <summary>
		/// Delete an image from an inspection item
		/// </summary>
		/// <param name="inspectionItemId">Id of the inspection item to remove the image from</param>
		/// <param name="imageId">The id of the image to remove</param>
		/// <response code="204">No content</response>
		/// <response code="404">Inspection item not found, or image not found to belong to inspection item</response>
		/// <response code="500">Unable to delete image</response>
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

		/// <summary>
		/// Set the visibility of the image
		/// </summary>
		/// <param name="inspectionItemId">Id of the inspection item the image belongs to</param>
		/// <param name="imageId">Id of the image to change the visibility of</param>
		/// <param name="isVisibleToCustomer">
		/// If true, this image will be displayed in the inspection report.
		/// If false, it will be hidden from the inspection report, making it for internal use only.
		/// </param>
		/// <response code="204">No content</response>
		/// <response code="404">Inspection item not found, or image not found to belong to inspection item</response>
		/// <response code="500">Unable to update visibility</response>
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
