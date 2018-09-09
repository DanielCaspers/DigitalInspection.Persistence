using System;
using System.Linq;
using System.Collections.Generic;
using DigitalInspectionNetCore21.Models.DbContexts;
using DigitalInspectionNetCore21.Models.Inspections;
using DigitalInspectionNetCore21.Models.Inspections.Joins;
using DigitalInspectionNetCore21.ViewModels.ChecklistItems;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalInspectionNetCore21.Controllers
{
	//[AuthorizeRoles(Roles.Admin)]
	[Route("[controller]")]
	public class ChecklistItemsController : BaseController, IRepositoryController<ChecklistItem, AddChecklistItemViewModel, EditChecklistItemViewModel>
	{
		public ChecklistItemsController(ApplicationDbContext db) : base(db)
		{
			ResourceName = "Checklist item";
		}

		[HttpGet("")]
		public ActionResult<IEnumerable<ChecklistItem>> GetAll()
		{
			var checklistItems = _context.ChecklistItems
				.OrderBy(ci => ci.Name)
				.ToList();

			return Json(checklistItems);
		}

		[HttpGet("{id}")]
		public ActionResult<ChecklistItem> GetById(Guid id)
		{
			var checklistItem = _context.ChecklistItems
				.Include(ci => ci.ChecklistItemTags)
					.ThenInclude(cit => cit.Tag)
				.Include(ci => ci.CannedResponses)
				.Include(ci => ci.Measurements)
				.SingleOrDefault(c => c.Id == id);

			if (checklistItem == null)
			{
				return NotFound();
			}
			else
			{
				checklistItem.Measurements = checklistItem.Measurements.OrderBy(m => m.Label).ToList();
				checklistItem.CannedResponses = checklistItem.CannedResponses.OrderBy(c => c.Response).ToList();

				return Json(checklistItem);
			}
		}

		[HttpPost("")]
		public ActionResult<ChecklistItem> Create([FromBody]AddChecklistItemViewModel request)
		{
			var checklistItem = new ChecklistItem()
			{
				Name = request.Name,
				CannedResponses = new List<CannedResponse>(),
				Measurements = new List<Measurement>()
			};
			checklistItem.ChecklistItemTags = request.TagIds.Select(tagId =>
			{
				var tag = _context.Tags.Find(tagId);
				return new ChecklistItemTag
				{
					Tag = tag,
					TagId = tagId,
					ChecklistItem = checklistItem,
					ChecklistItemId = checklistItem.Id
				};
			}).ToList();

			_context.ChecklistItems.Add(checklistItem);

			_context.SaveChanges();

			var createdUri = new Uri(HttpContext.Request.Path, UriKind.Relative);
			return Created(createdUri, checklistItem);
		}

		[HttpPut("{id}")]
		public ActionResult<ChecklistItem> Update(Guid id, [FromBody]EditChecklistItemViewModel vm)
		{
			var checklistItemInDb = _context.ChecklistItems.SingleOrDefault(c => c.Id == id);
			if(checklistItemInDb == null)
			{
				return NotFound();
			}

			// Duplicating database entries bug - MUST BE DONE BEFORE PROP CHANGES 
			// http://stackoverflow.com/a/22389505/2831961
			foreach (var measurement in checklistItemInDb.Measurements)
			{
				_context.Measurements.Attach(measurement);
			}

			foreach (var cannedResponse in checklistItemInDb.CannedResponses)
			{
				_context.CannedResponses.Attach(cannedResponse);
			}

			var tags = checklistItemInDb.ChecklistItemTags?.Select(joinItem => joinItem.Tag);
			if (tags != null)
			{
				foreach (var tag in tags)
				{
					_context.Tags.Attach(tag);
				}
			}

			foreach (var measurementInVm in vm.ChecklistItem.Measurements)
			{
				var measurementInDb = checklistItemInDb.Measurements.SingleOrDefault(cm => cm.Id == measurementInVm.Id);
				if (measurementInDb != null)
				{
					measurementInDb.Label = measurementInVm.Label;
					measurementInDb.Unit = measurementInVm.Unit;
					measurementInDb.MinValue = measurementInVm.MinValue;
					measurementInDb.MaxValue = measurementInVm.MaxValue;
					measurementInDb.StepSize = measurementInVm.StepSize;
				}
			}

			foreach (var cannedResponseInVm in vm.ChecklistItem.CannedResponses)
			{
				var cannedResponseInDb = checklistItemInDb.CannedResponses.SingleOrDefault(cr => cr.Id == cannedResponseInVm.Id);
				if (cannedResponseInDb != null)
				{
					cannedResponseInDb.Response = cannedResponseInVm.Response;
					cannedResponseInDb.LevelsOfConcern = cannedResponseInVm.LevelsOfConcern;
					cannedResponseInDb.Url = cannedResponseInVm.Url;
					cannedResponseInDb.Description = cannedResponseInVm.Description;
				}
			}

			checklistItemInDb.Name = vm.ChecklistItem.Name;
			// FIXME DJC EF Many2Many - Uncertain this will work. May need to attach ChecklistItemTags since things may be getting duplicated, or not removed when not reassigned
			checklistItemInDb.ChecklistItemTags = _context.Tags
				.Where(t => vm.SelectedTagIds.Contains(t.Id))
				.Select(t => new ChecklistItemTag
				{
					ChecklistItem = checklistItemInDb,
					ChecklistItemId = checklistItemInDb.Id,
					Tag = t,
					TagId = t.Id
				})
				.ToList();

			_context.SaveChanges();

			return NoContent();
		}

		[HttpDelete("{id}")]
		public NoContentResult Delete(Guid id)
		{
			var checklistItemInDb = _context.ChecklistItems.Find(id);

			if (checklistItemInDb != null)
			{
				// TODO: DJC Should cascade delete work from checklistitem to measurement and canned response
				foreach (var measurement in _context.Measurements)
				{
					if (measurement.ChecklistItemId == id)
					{
						_context.Measurements.Remove(measurement);
					}
				}

				// TODO: DJC Should cascade delete work from checklistitem to measurement and canned response
				foreach (var cannedResponse in _context.CannedResponses)
				{
					if (cannedResponse.ChecklistItemId == id)
					{
						_context.CannedResponses.Remove(cannedResponse);
					}
				}

				_context.ChecklistItems.Remove(checklistItemInDb);
				_context.SaveChanges();
			}

			return NoContent();
		}

		[HttpPost("{id}/Measurements")]
		public ActionResult AddMeasurement(Guid id)
		{
			var checklistItemInDb = _context.ChecklistItems.Find(id);

			if (checklistItemInDb == null)
			{
				return NotFound();
			}

			var measurement = new Measurement();
			checklistItemInDb.Measurements.Add(measurement);

			_context.SaveChanges();

			return Ok(measurement);
		}

		[HttpDelete("{checklistItemId}/Measurements/{measurementId}")]
		public ActionResult DeleteMeasurement(Guid checklistItemId, Guid measurementId)
		{
			var checklistItemInDb = _context.ChecklistItems
				.Include(ci => ci.Measurements)
				.Single(ci => ci.Id == checklistItemId);

			if (checklistItemInDb != null)
			{
				var measurementToRemove = checklistItemInDb.Measurements.Single(m => m.Id == measurementId);
				checklistItemInDb.Measurements.Remove(measurementToRemove);

				// Uncomment this line if it is desired to remove all notions of this measurement from the APP.
				// Leaving this commented out only removes its association from a checklist for new items, but allows
				// old inspections to remain historically accurate. 
				//_context.Measurements.Remove(measurementToRemove);
				_context.SaveChanges();
			}

			return NoContent();
		}

		[HttpPost("{id}/CannedResponses")]
		public ActionResult AddCannedResponse(Guid id)
		{
			var checklistItemInDb = _context.ChecklistItems.Find(id);

			if (checklistItemInDb == null)
			{
				return NotFound();
			}

			var cannedResponse = new CannedResponse()
			{
				Response = "A new response"
			};
			checklistItemInDb.CannedResponses.Add(cannedResponse);

			_context.SaveChanges();

			return Ok(cannedResponse);
		}

		[HttpDelete("{checklistItemId}/CannedResponses/{cannedResponseId}")]
		public ActionResult DeleteCannedResponse(Guid checklistItemId, Guid cannedResponseId)
		{
			var checklistItemInDb = _context.ChecklistItems
				.Include(ci => ci.CannedResponses)
				.Single(ci => ci.Id == checklistItemId);

			if (checklistItemInDb != null)
			{
				var cannedResponseToRemove = checklistItemInDb.CannedResponses.Single(m => m.Id == cannedResponseId);
				checklistItemInDb.CannedResponses.Remove(cannedResponseToRemove);

				// Uncomment this line if it is desired to remove all notions of this canned response from the APP.
				// Leaving this commented out only removes its association from a checklist for new items, but allows
				// old inspections to remain historically accurate. 
				//_context.CannedResponses.Remove(cannedResponseToRemove);
				_context.SaveChanges();
			}

			return NoContent();
		}
	}
}
