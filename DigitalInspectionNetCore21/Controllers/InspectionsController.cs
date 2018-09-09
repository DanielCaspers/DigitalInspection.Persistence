using DigitalInspectionNetCore21.Models.Web;
using DigitalInspectionNetCore21.ViewModels;
using DigitalInspectionNetCore21.Services;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using DigitalInspectionNetCore21.Models.Orders;
using System.IO;
using DigitalInspectionNetCore21.Models.DbContexts;
using DigitalInspectionNetCore21.Models.Inspections;
using DigitalInspectionNetCore21.Models.Inspections.Joins;
using DigitalInspectionNetCore21.Models.Inspections.Reports;
using DigitalInspectionNetCore21.Services.Core;
using DigitalInspectionNetCore21.Services.Web;
using DigitalInspectionNetCore21.ViewModels.Inspections;
using DigitalInspectionNetCore21.ViewModels.TabContainers;
using DigitalInspectionNetCore21.ViewModels.VehicleHistory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace DigitalInspectionNetCore21.Controllers
{
	public class InspectionsController : BaseController
	{
		private static readonly string IMAGE_DIRECTORY = "Inspections";
		private static readonly string _subresource = "Inspection Item";

		public InspectionsController(ApplicationDbContext db) : base(db)
		{
			ResourceName = "InspectionsController";
		}

		// [AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public PartialViewResult Index(string workOrderId, Guid checklistId, Guid? tagId)
		{
			return PartialView(GetInspectionViewModel(workOrderId, checklistId, tagId));
		}

		[AllowAnonymous]
		public JsonResult InspectionIdForOrder(string workOrderId)
		{
			var id = _context.Inspections
				.SingleOrDefault(i => i.WorkOrderId == workOrderId)
				?.Id;

			return Json(id);
		}

		[AllowAnonymous]
		public JsonResult ReportForOrder(string workOrderId, bool grouped = false, bool includeUnknown = false)
		{
			var inspectionItems = GetInspectionItemsFromDb()
				.Single(i => i.WorkOrderId == workOrderId)
				.InspectionItems;

			return BuildInspectionReportInternal(inspectionItems, grouped, includeUnknown);
		}

		[AllowAnonymous]
		public JsonResult Report(Guid inspectionId, bool grouped = false, bool includeUnknown = false)
		{
			var inspectionItems = GetInspectionItemsFromDb()
				.Single(i => i.Id == inspectionId)
				.InspectionItems;

			return BuildInspectionReportInternal(inspectionItems, grouped, includeUnknown);
		}

		private IIncludableQueryable<Inspection, CannedResponse> GetInspectionItemsFromDb()
		{
			return _context.Inspections
					.Include(i => i.InspectionItems)
						.ThenInclude(ii => ii.InspectionImages)
					.Include(i => i.InspectionItems)
						.ThenInclude(ii => ii.ChecklistItem)
							.ThenInclude(ii => ii.ChecklistItemTags)
					.Include(i => i.InspectionItems)
						.ThenInclude(ii => ii.InspectionMeasurements)
							.ThenInclude(im => im.Measurement)
					.Include(i => i.InspectionItems)
						.ThenInclude(ii => ii.InspectionItemCannedResponses)
							.ThenInclude(iirc => iirc.CannedResponse);
		}

		[HttpPost]
		[AllowAnonymous]
		public ActionResult Delete(string workOrderId)
		{
			if (Request.Headers["DigitalInspectionNetCore21-AppKey"] !=
			    ConfigurationManager.AppSettings.Get("DigitalInspectionNetCore21-AppKey"))
			{
				return Json($"Not authorized to delete inspection for {workOrderId}");
			}

			var inspection = _context.Inspections.SingleOrDefault(i => i.WorkOrderId == workOrderId);
			if (inspection == null)
			{
				return new EmptyResult();
			}

			InspectionService.DeleteInspection(_context, inspection);

			return new EmptyResult();
		}

		[HttpPost]
		// [AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public ActionResult Condition(Guid inspectionItemId, RecommendedServiceSeverity inspectionItemCondition)
		{
			// Save Condition
			var inspectionItemInDb = _context.InspectionItems.SingleOrDefault(item => item.Id == inspectionItemId);

			if (inspectionItemInDb == null)
			{
				return NotFound();
			}

			if (InspectionService.UpdateInspectionItemCondition(_context, inspectionItemInDb, inspectionItemCondition) == false)
			{
				return StatusCode(500);
			}

			// Prepare updated multiselect list for client
			var checklistItem = _context.ChecklistItems.Single(ci => ci.Id == inspectionItemInDb.ChecklistItem.Id);
			var filteredCRs = checklistItem.CannedResponses.Where(cr => cr.LevelsOfConcern.Contains(inspectionItemCondition));

			// Options may be selected in the case where we haven't changed to a new condition
			var options = filteredCRs.Select(cr => new
			{
				label = cr.Response,
				title = cr.Response,
				value = cr.Id,
				selected = inspectionItemInDb.InspectionItemCannedResponses.Any(joinItem => joinItem.CannedResponse.Id == cr.Id)
			});

			var response = new
			{
				filteredCannedResponses = options,
				checklistItemId = checklistItem.Id
			};
			return Json(response);
		}

		[HttpPost]
		// [AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public ActionResult CannedResponse(Guid inspectionItemId, InspectionDetailViewModel vm)
		{
			var inspectionItemInDb = _context.InspectionItems.Single(item => item.Id == inspectionItemId);

			IList<Guid> selectedCannedResponseIds = vm.Inspection.InspectionItems.Single(ii => ii.Id == inspectionItemId).SelectedCannedResponseIds;

			InspectionService.UpdateInspectionItemCannedResponses(_context, inspectionItemInDb, selectedCannedResponseIds);
			return NoContent();
		}

		[HttpPost]
		// [AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public ActionResult IsCustomerConcern(Guid inspectionItemId, bool isCustomerConcern)
		{
			var inspectionItemInDb = _context.InspectionItems.SingleOrDefault(item => item.Id == inspectionItemId);

			if (inspectionItemInDb == null)
			{
				return NotFound();
			}

			if (InspectionService.UpdateIsCustomerConcern(_context, inspectionItemInDb, isCustomerConcern))
			{
				return NoContent();
			}

			return StatusCode(500);
		}

		[HttpPost]
		// [AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public ActionResult ItemNote(AddInspectionItemNoteViewModel itemNoteVm)
		{
			var inspectionItemInDb = _context.InspectionItems.SingleOrDefault(item => item.Id == itemNoteVm.InspectionItem.Id);

			if (inspectionItemInDb == null)
			{
				return NotFound();
			}

			if (InspectionService.UpdateInspectionItemNote(_context, inspectionItemInDb, itemNoteVm.Note))
			{
				return NoContent();
			}

			return StatusCode(500);
		}

		//[HttpPost]
		//// [AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		//public ActionResult WorkOrderNote(AddInspectionWorkOrderNoteViewModel workOrderNoteVm)
		//{
		//	if (workOrderNoteVm.Note == null)
		//	{
		//		workOrderNoteVm.Note = "";
		//	}

		//	// https://msdn.microsoft.com/en-us/library/tabh47cf(v=vs.110).aspx
		//	// NOTE: Cannot use Environment.NewLine since the filter will be less strict on Mono. 
		//	IList<string> returnCarriageSeparatedNotes = workOrderNoteVm.Note.GroupByLineEnding();

		//	var task = Task.Run(async () => {
		//		return await WorkOrderService.SaveWorkOrderNote(CurrentUserClaims, workOrderNoteVm.WorkOrderId, GetCompanyNumber(), returnCarriageSeparatedNotes);
		//	});
		//	// Force Synchronous run for Mono to work. See Issue #37
		//	task.Wait();

		//	if (task.Result.IsSuccessStatusCode)
		//	{
		//		return new EmptyResult();
		//	}

		//	return PartialView("Toasts/_Toast", ToastService.WorkOrderError(task.Result));
		//}

		[HttpPost]
		// [AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public ActionResult Measurements(AddMeasurementViewModel MeasurementsVM)
		{
			var inspectionItemInDb = _context.InspectionItems.SingleOrDefault(item => item.Id == MeasurementsVM.InspectionItem.Id);

			if (inspectionItemInDb == null)
			{
				return NotFound();
			}

			if (InspectionService.UpdateInspectionItemMeasurements(_context, inspectionItemInDb, MeasurementsVM.InspectionItem.InspectionMeasurements))
			{
				return NoContent();
			}

			return StatusCode(500);
		}

		[HttpPost]
		// [AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public ActionResult Photo(UploadInspectionPhotosViewModel photoVM)
		{
			var inspectionItemInDb = _context.InspectionItems.SingleOrDefault(item => item.Id == photoVM.InspectionItem.Id);

			if (inspectionItemInDb == null)
			{
				return NotFound();
			}
			// New guid is used as a random prefix to the filename to ensure uniqueness
			Image imageDto = ImageService.SaveImage(photoVM.Picture, new[] { IMAGE_DIRECTORY, photoVM.WorkOrderId, photoVM.InspectionItem.Id.ToString() }, Guid.NewGuid().ToString(), false);

			if (InspectionService.AddInspectionItemImage(_context, inspectionItemInDb, imageDto))
			{
				return NoContent();
			}

			return StatusCode(500);
		}

		[HttpPost]
		// [AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public ActionResult DeletePhoto(Guid imageId, Guid checklistId, Guid? tagId, string workOrderId)
		{
			var image = _context.InspectionImages.SingleOrDefault(inspectionImage => inspectionImage.Id == imageId);

			if (image == null)
			{
				return NotFound();
			}

			var inspectionItemInDb = _context.InspectionItems.SingleOrDefault(item => item.Id == image.InspectionItem.Id);

			if (inspectionItemInDb == null)
			{
				return NotFound();
			}

			ImageService.DeleteImage(image);

			if (InspectionService.DeleteInspectionItemImage(_context, image))
			{
				return NoContent();
			}

			return StatusCode(500);
		}

		[HttpPost]
		// [AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public ActionResult IsPhotoVisibleToCustomer(Guid inspectionItemId, Guid inspectionImageId, bool isVisibleToCustomer)
		{
			var inspectionItemInDb = _context.InspectionItems.SingleOrDefault(item => item.Id == inspectionItemId);
			if (inspectionItemInDb == null)
			{
				return NotFound();
			}

			var inspectionImage = inspectionItemInDb.InspectionImages.SingleOrDefault(ii => ii.Id == inspectionImageId);
			if (inspectionImage == null)
			{
				return NotFound();
			}

			if (InspectionService.UpdateInspectionImageVisibility(_context, inspectionImage, isVisibleToCustomer))
			{
				return NoContent();
			}

			return StatusCode(500);
		}

		[HttpPost]
		// [AuthorizeRoles(Roles.Admin, Roles.User, Roles.LocationManager, Roles.ServiceAdvisor, Roles.Technician)]
		public PartialViewResult GetViewInspectionPhotosDialog(Guid inspectionItemId, Guid checklistItemId, Guid checklistId, Guid? tagId, string workOrderId)
		{
			var checklistItem = _context.ChecklistItems.SingleOrDefault(ci => ci.Id == checklistItemId);
			var inspectionItem = _context.InspectionItems.Single(item => item.Id == inspectionItemId);

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

		private InspectionDetailViewModel GetInspectionViewModel(string workOrderId, Guid checklistId, Guid? tagId)
		{
			var workOrderResponse = GetWorkOrderResponse(workOrderId);
			var workOrder = workOrderResponse.Entity;
			ToastViewModel toast = null;

			var checklist = _context.Checklists.Single(c => c.Id == checklistId);

			if (workOrderResponse.IsSuccessStatusCode == false)
			{
				toast = ToastService.WorkOrderError(workOrderResponse);
			}
			else if (checklist == null)
			{
				toast = ToastService.ResourceNotFound(ResourceName, ToastActionType.NavigateBack);
			}

			// Sort all canned responses by response
			checklist.ChecklistChecklistItems.ToList().ForEach(joinItem =>
			{
				joinItem.ChecklistItem.CannedResponses = joinItem.ChecklistItem.CannedResponses.OrderBy(cr => cr.Response).ToList();
			});

			var inspection = InspectionService.GetOrCreateInspection(_context, workOrderId, checklist);

			// Filter inspection items
			Func<InspectionItem, bool> filterByChecklistsAndTags = ii => ii.ChecklistItem.ChecklistChecklistItems.Any(joinItem => joinItem.Checklist.Id == checklistId) && ii.ChecklistItem.ChecklistItemTags.Select(joinItem => joinItem.Tag).Any(t => t.Id == tagId);
			Func<InspectionItem, bool> filterByChecklists = ii => ii.ChecklistItem.ChecklistChecklistItems.Any(joinItem => joinItem.Checklist.Id == checklistId);

			inspection.InspectionItems = inspection.InspectionItems
				.Where(tagId.HasValue ? filterByChecklistsAndTags : filterByChecklists)
				.OrderBy(ii => ii.ChecklistItem.Name)
				.ToList();

			foreach(var inspectionItem in inspection.InspectionItems)
			{
				inspectionItem.SelectedCannedResponseIds = inspectionItem.InspectionItemCannedResponses.Select(joinItem => joinItem.CannedResponse.Id).ToList();
			}

			return new InspectionDetailViewModel
			{
				WorkOrder = workOrder,
				Checklist = checklist,
				Inspection = inspection,
				Toast = toast,
				AddMeasurementVM = new AddMeasurementViewModel(),
				AddInspectionWorkOrderNoteVm = new AddInspectionWorkOrderNoteViewModel(),
				AddInspectionItemNoteVm = new AddInspectionItemNoteViewModel(),
				UploadInspectionPhotosVM = new UploadInspectionPhotosViewModel {
					WorkOrderId = workOrderId
				},
				ViewInspectionPhotosVM = new ViewInspectionPhotosViewModel(),
				VehicleHistoryVM = new VehicleHistoryViewModel(),
				ConfirmInspectionCompleteViewModel = new ConfirmDialogViewModel
				{
					Title = "Mark inspection as completed?",
					Body = "This cannot be undone without a service advisor.",
					AffirmativeActionText = "I'm done!",
					CancelActionText = "Not yet"
				},
				ScrollableTabContainerVM = GetScrollableTabContainerViewModel(tagId),
				FilteringTagId = tagId
			};
		}

		// TODO remove from new DI
		private ScrollableTabContainerViewModel GetScrollableTabContainerViewModel(Guid? tagId)
		{
			// Construct tabs based on current selection
			var applicableTags = _context.Tags
				.Where(t => t.IsVisibleToEmployee)
				.OrderBy(t => t.Name)
				.ToList();

			IList<ScrollableTab> tabs = new List<ScrollableTab>
			{
				new ScrollableTab {
					paneId = null,
					title = "All tags"
				}
			};

			foreach (Tag tag in applicableTags)
			{
				tabs.Add(new ScrollableTab { paneId = tag.Id, title = tag.Name });
			}

			if (tagId.HasValue)
			{
				// Make matching tag active
				tabs.Where(tab => tab.paneId == tagId).ElementAt(0).active = true;
			}
			else
			{
				// Make default (All tags) appear active
				tabs.ElementAt(0).active = true;
			}

			return new ScrollableTabContainerViewModel
			{
				Tabs = tabs
			};
		}

		private JsonResult BuildInspectionReportInternal(IEnumerable<InspectionItem> unfilteredInspectionItems, bool grouped, bool includeUnknown)
		{
			var applicableTags = _context.Tags
				.Where(t => t.IsVisibleToCustomer)
				.Select(t => t.Id)
				.ToList();

			// Only show inspection items which correspond to one or more customer visible tags
			var inspectionItems = unfilteredInspectionItems
				.Where(ii => ii.ChecklistItem.ChecklistItemTags
					.Select(joinItem => joinItem.TagId)
					.Intersect(applicableTags)
					.Any()
				).ToList();

			if (!includeUnknown)
			{
				// Only show inspection items which have had a marked condition
				inspectionItems = inspectionItems.Where(ii => ii.Condition != RecommendedServiceSeverity.UNKNOWN).ToList();
			}

			string baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";

			if (grouped)
			{
				var inspectionReportGroups = inspectionItems
					.GroupBy(ii =>
						ii.ChecklistItem.ChecklistItemTags
							.Select(joinItem => joinItem.Tag)
							.Where(t => t.IsVisibleToCustomer)
							.Select(t => t.Name)
							.First()
					)
					.OrderBy(ig => ig.OrderBy(ii => ii.Condition).First().Condition)
					.Select(ig => new InspectionReportGroup(ig, baseUrl))
					.ToList();

				return Json(inspectionReportGroups);
			}
			else
			{
				var inspectionReportItems = inspectionItems
					.OrderBy(ii => ii.Condition)
					.Select(ii => new InspectionReportItem(ii, baseUrl))
					.ToList();

				return Json(inspectionReportItems);
			}
		}

		private HttpResponse<WorkOrder> GetWorkOrderResponse(string workOrderId)
		{
			var task = Task.Run(async () => {
				return await WorkOrderService.GetWorkOrder(CurrentUserClaims, workOrderId, GetCompanyNumber(), false);
			});
			// Force Synchronous run for Mono to work. See Issue #37
			task.Wait();

			return task.Result;
		}
	}
}
