using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DigitalInspectionNetCore21.Models.DbContexts;
using DigitalInspectionNetCore21.Models.Inspections;
using DigitalInspectionNetCore21.Models.Inspections.Joins;
using DigitalInspectionNetCore21.Models.Orders;
using Microsoft.EntityFrameworkCore;

namespace DigitalInspectionNetCore21.Services.Core
{
	public static class InspectionService
	{
		#region Inspection CRUD

		public static Inspection GetOrCreateInspection(ApplicationDbContext ctx, string workOrderId, Checklist checklist)
		{
			var inspection = GetOrCreateInspectionInternal(ctx, workOrderId, checklist);
			inspection = GetOrCreateInspectionItems(ctx, checklist, inspection);
			return GetOrCreateInspectionMeasurements(ctx, inspection);
		}

		public static bool DeleteInspection(ApplicationDbContext ctx, Inspection inspection)
		{
			var inspectionMeasurements = ctx.InspectionMeasurements.Where(im => im.InspectionItem.Inspection.Id == inspection.Id);
			ctx.InspectionMeasurements.RemoveRange(inspectionMeasurements);

			var inspectionImages = ctx.InspectionImages.Where(ii => ii.InspectionItem.Inspection.Id == inspection.Id).ToList();
			inspectionImages.ForEach(ImageService.DeleteImage);

			// TODO: Revise service when dependency injection framework is in use, and abstract into a service
			// FIXME DJC Requires refactor 
			// https://www.mikesdotnetting.com/article/302/server-mappath-equivalent-in-asp-net-core
			string path = ""; // HttpContext.Current.Server.MapPath($"~/Uploads/Inspections/{inspection.WorkOrderId}");
			try
			{
				Directory.Delete(path, true);
			}
			catch (IOException)
			{
				Directory.Delete(path, true);
			}
			catch (UnauthorizedAccessException)
			{
				Directory.Delete(path, true);
			}

			ctx.InspectionImages.RemoveRange(inspectionImages);

			var inspectionItems = ctx.InspectionItems.Where(ii => ii.Inspection.Id == inspection.Id);
			ctx.InspectionItems.RemoveRange(inspectionItems);

			ctx.Inspections.Remove(inspection);

			return TrySave(ctx);
		}

		#endregion

		#region InspectionItem CRUD

		// Return value indicates success of operation
		public static bool UpdateInspectionItemCondition(
			ApplicationDbContext ctx,
			InspectionItem inspectionItem,
			RecommendedServiceSeverity inspectionItemCondition)
		{
			// Only change condition and canned response if different from previous condition
			if (inspectionItem.Condition == inspectionItemCondition)
			{
				return true;
			}

			inspectionItem.Condition = inspectionItemCondition;

			// Clear canned response IDs when switching conditions.
			// This is because otherwise, the inspection table's select box and the DB (and thus the report) can get out of sync
			foreach (var inspectionItemCannedResponse in inspectionItem.InspectionItemCannedResponses)
			{
				var cannedResponse = inspectionItemCannedResponse.CannedResponse;
				ctx.CannedResponses.Attach(cannedResponse);
			}
			// FIXME DJC EF Many2Many - Determine if this still works. Its possible EF won't see this as clearing with the new many-many join style
			inspectionItem.InspectionItemCannedResponses = new List<InspectionItemCannedResponse>();

			return TrySave(ctx);
		}

		public static bool UpdateInspectionItemCannedResponses(
			ApplicationDbContext ctx,
			InspectionItem inspectionItem,
			IList<Guid> selectedCannedResponseIds)
		{
			foreach (var inspectionItemCannedResponse in inspectionItem.InspectionItemCannedResponses)
			{
				var cannedResponse = inspectionItemCannedResponse.CannedResponse;
				ctx.CannedResponses.Attach(cannedResponse);
			}

			inspectionItem.InspectionItemCannedResponses = 
				selectedCannedResponseIds
					.Select(crId => ctx.CannedResponses.Single(cr => cr.Id == crId))
					// FIXME DJC EF Many2Many - Had to widen from just adjusting canned responses, so this may impact property tracking
					.Select(cr => new InspectionItemCannedResponse
					{
						InspectionItem = inspectionItem,
						CannedResponse = cr,
						InspectionItemId = inspectionItem.Id,
						CannedResponseId = cr.Id
					})
					.ToList();

			return TrySave(ctx);
		}

		public static bool UpdateIsCustomerConcern(
			ApplicationDbContext ctx,
			InspectionItem inspectionItem,
			bool isCustomerConcern)
		{
			inspectionItem.IsCustomerConcern = isCustomerConcern;

			return TrySave(ctx);
		}

		public static bool UpdateInspectionItemNote(
			ApplicationDbContext ctx,
			InspectionItem inspectionItem,
			string note)
		{
			inspectionItem.Note = note;
			return TrySave(ctx);
		}

		public static bool UpdateInspectionItemMeasurements(
			ApplicationDbContext ctx,
			InspectionItem inspectionItem,
			IEnumerable<InspectionMeasurement> inspectionMeasurements)
		{
			foreach (var inspectionMeasurement in inspectionMeasurements)
			{
				ctx.InspectionMeasurements
					.Single(im => im.Id == inspectionMeasurement.Id)
					.Value = inspectionMeasurement.Value;
			}

			return TrySave(ctx);
		}

		public static bool AddInspectionItemImage(
			ApplicationDbContext ctx,
			InspectionItem inspectionItem,
			Image image)
		{
			var inspectionImage = new InspectionImage
			{
				Id = image.Id,
				Title = image.Title,
				CreatedDate = image.CreatedDate,
				ImageUrl = image.ImageUrl,
				InspectionItem = inspectionItem
			};

			inspectionItem.InspectionImages.Add(inspectionImage);
			ctx.InspectionImages.Add(inspectionImage);

			return TrySave(ctx);
		}

		public static bool DeleteInspectionItemImage(
			ApplicationDbContext ctx,
			InspectionImage image)
		{
			ctx.InspectionImages.Remove(image);
			return TrySave(ctx);
		}

		public static bool UpdateInspectionImageVisibility(
			ApplicationDbContext ctx,
			InspectionImage image,
			bool isVisibleToCustomer)
		{
			image.IsVisibleToCustomer = isVisibleToCustomer;

			return TrySave(ctx);
		}

		#endregion

		#region Private Helpers

		/**
		 * Gets inspection from DB if already exists, or create one in the DB and then return it. 
		 */
		private static Inspection GetOrCreateInspectionInternal(
			ApplicationDbContext ctx,
			string workOrderId,
			Checklist checklist)
		{
			var inspection = ctx.Inspections
				.Include(i => i.ChecklistItemInspections)
					.ThenInclude(i => i.ChecklistItem)
				.Include(i => i.InspectionItems)
					.ThenInclude(ii => ii.InspectionImages)
				.Include(i => i.InspectionItems)
					.ThenInclude(ii => ii.InspectionMeasurements)
				.Include(i => i.InspectionItems)
					.ThenInclude(ii => ii.InspectionItemCannedResponses)
				.Include(i => i.ChecklistInspections)
					.ThenInclude(ci => ci.Checklist)
				.SingleOrDefault(i => i.WorkOrderId == workOrderId);

			if (inspection == null)
			{
				inspection = new Inspection
				{
					WorkOrderId = workOrderId,
				};
				// FXIME DJC EF Many2Many - This relationship no longer works
				inspection.ChecklistInspections = new List<ChecklistInspection>
				{
					new ChecklistInspection
					{
						Checklist = checklist,
						ChecklistId = checklist.Id,
						Inspection = inspection,
						InspectionId = new Guid()
					}
				};

				ctx.Inspections.Add(inspection);
			}
			// An inspection exists, but this checklist has not yet been performed on it
			// FXIME DJC EF Many2Many - This relationship may no longer work
			else if (inspection.ChecklistInspections.Any(joinItem => joinItem.Checklist.Id == checklist.Id) == false)
			{
				inspection.ChecklistInspections.Add(new ChecklistInspection
				{
					Checklist = checklist,
					ChecklistId = checklist.Id,
					Inspection = inspection,
					InspectionId = inspection.Id
				});
			}

			TrySave(ctx);

			return inspection;
		}

		/**
		 * Gets all inspectionitems from DB if they already exists, and creates them if they don't already 
		 */
		private static Inspection GetOrCreateInspectionItems(
			ApplicationDbContext ctx,
			Checklist checklist,
			Inspection inspection)
		{
			foreach (var ci in checklist.ChecklistChecklistItems.Select(joinItem => joinItem.ChecklistItem))
			{
				var inspectionItem = inspection.InspectionItems.SingleOrDefault(item => item.ChecklistItem.Id == ci.Id);
				if (inspectionItem == null)
				{
					inspectionItem = new InspectionItem
					{
						ChecklistItem = ci,
						Inspection = inspection
					};

					inspection.InspectionItems.Add(inspectionItem);
					ctx.InspectionItems.Add(inspectionItem);
				}
			}

			TrySave(ctx);

			return inspection;
		}

		/**
		 * Gets all inspectionmeasurements from DB if they already exist, and creates them if they don't already 
		 */
		private static Inspection GetOrCreateInspectionMeasurements(ApplicationDbContext ctx, Inspection inspection)
		{
			foreach (var item in inspection.InspectionItems)
			{
				var measurements = ctx.Measurements.Where(m => m.ChecklistItem.Id == item.ChecklistItem.Id).ToList();

				foreach (var measurement in measurements)
				{
					if (item.InspectionMeasurements.Any(im => im.Measurement.Id == measurement.Id) == false)
					{
						var inspectionMeasurement = new InspectionMeasurement
						{
							InspectionItem = item,
							Measurement = measurement,
							Value = null
						};
						ctx.InspectionMeasurements.Add(inspectionMeasurement);
					}
				}
			}

			TrySave(ctx);

			return ctx.Inspections.Single(i => i.Id == inspection.Id);
		}

		private static bool TrySave(DbContext ctx)
		{
			bool wasSuccessful = false;
			try
			{
				ctx.SaveChanges();
				wasSuccessful = true;
			}
			catch (Exception dbEx)
			{
				// ExceptionHandlerService.HandleException(dbEx);
			}
			// FIXME DJC EF Validation - Appears to be different https://stackoverflow.com/questions/46430619/net-core-2-ef-core-error-handling-save-changes
			//try
			//{
			//	ctx.SaveChanges();
			//	wasSuccessful = true;
			//}
			//catch (DbEntityValidationException dbEx)
			//{
			//	ExceptionHandlerService.HandleException(dbEx);
			//}

			return wasSuccessful;
		}

		#endregion
	}

}
