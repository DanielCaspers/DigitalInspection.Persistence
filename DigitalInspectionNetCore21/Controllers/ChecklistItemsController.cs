using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using DigitalInspectionNetCore21.Models.DbContexts;
using DigitalInspectionNetCore21.Models.Inspections;
using DigitalInspectionNetCore21.Models.Inspections.Joins;
using DigitalInspectionNetCore21.Models.Web.Inspections;
using DigitalInspectionNetCore21.Services.Core.Interfaces;
using DigitalInspectionNetCore21.ViewModels.ChecklistItems;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalInspectionNetCore21.Controllers
{
	//[AuthorizeRoles(Roles.Admin)]
	[Route("[controller]")]
	public class ChecklistItemsController : BaseController
	{
		private readonly IChecklistItemRepository _checklistItemRepository;
		private readonly ITagRepository _tagRepository;

		public ChecklistItemsController(ApplicationDbContext db, IChecklistItemRepository checklistItemRepository, ITagRepository tagRepository) : base(db)
		{
			_checklistItemRepository = checklistItemRepository;
			_tagRepository = tagRepository;
		}

		[HttpGet("")]
		public ActionResult<IEnumerable<ChecklistItemSummaryResponse>> GetAll()
		{
			var checklistItems = _checklistItemRepository.GetAll().ToList();

			var checklistItemSummaryResponses = Mapper.Map<IEnumerable<ChecklistItemSummaryResponse>>(checklistItems);

			return Json(checklistItemSummaryResponses);
		}

		[HttpGet("{id}")]
		public ActionResult<ChecklistItemResponse> GetById(Guid id)
		{
			var checklistItem = _checklistItemRepository.GetById(id);

			if (checklistItem == null)
			{
				return NotFound();
			}
			else
			{
				var checklistItemResponse = Mapper.Map<ChecklistItemResponse>(checklistItem);
				return Json(checklistItemResponse);
			}
		}

		[Obsolete("Use only for Legacy .NET Framework App")]
		[HttpGet("{id}/Edit")]
		public ActionResult<EditChecklistItemViewModel> EditById(Guid id)
		{
			var checklistItem = _checklistItemRepository.GetById(id);

			if (checklistItem == null)
			{
				return NotFound();
			}
			else
			{
				var tags = _tagRepository.GetAll().ToList();
				var selectedTagIds = checklistItem.ChecklistItemTags.Select(cit => cit.TagId);
				var viewModel = new EditChecklistItemViewModel
				{
					ChecklistItem = Mapper.Map<ChecklistItemResponse>(checklistItem),
					Tags = Mapper.Map<IList<TagResponse>>(tags),
					SelectedTagIds = selectedTagIds
				};
				return Json(viewModel);
			}
		}

		[HttpPost("")]
		public ActionResult<ChecklistItemResponse> Create([FromBody]AddChecklistItemViewModel request)
		{
			var checklistItem = new ChecklistItem()
			{
				Name = request.Name
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

			var checklistItemResponse = Mapper.Map<ChecklistItemResponse>(checklistItem);

			var createdUri = new Uri(HttpContext.Request.Path, UriKind.Relative);
			return Created(createdUri, checklistItemResponse);
		}

		[HttpPut("{id}")]
		public ActionResult Update(Guid id, [FromBody]EditChecklistItemViewModel vm)
		{
			// TODO Determine if Find() will help with getting rid of context attach.
			var checklistItemInDb = _context.ChecklistItems
				.Include(ci => ci.ChecklistItemTags)
					.ThenInclude(cit => cit.Tag)
				.Include(ci => ci.CannedResponses)
				.Include(ci => ci.Measurements)
				.SingleOrDefault(ci => ci.Id == id);

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
			var checklistItemInDb = _context.ChecklistItems
				.Include(ci => ci.ChecklistItemTags)
					.ThenInclude(cit => cit.Tag)
				.Include(ci => ci.CannedResponses)
				.Include(ci => ci.Measurements)
				.SingleOrDefault(ci => ci.Id == id);

			if (checklistItemInDb != null)
			{
				
				foreach (var measurement in _context.Measurements.Where(m => m.ChecklistItemId == id))
				{
					_context.Measurements.Remove(measurement);
				}

				foreach (var cannedResponse in _context.CannedResponses.Where(cr => cr.ChecklistItemId == id))
				{
					_context.CannedResponses.Remove(cannedResponse);
				}

				_context.ChecklistItems.Remove(checklistItemInDb);
				_context.SaveChanges();
			}
			return NoContent();
		}

		[HttpPost("{id}/Measurements")]
		public ActionResult AddMeasurement(Guid id)
		{
			var checklistItem = _context.ChecklistItems.Find(id);

			if (checklistItem == null)
			{
				return NotFound();
			}

			var measurement = new Measurement();
			checklistItem.Measurements.Add(measurement);

			_context.SaveChanges();

			var measurementResponse = Mapper.Map<MeasurementResponse>(measurement);

			return Ok(measurementResponse);
		}

		[HttpDelete("{checklistItemId}/Measurements/{measurementId}")]
		public ActionResult DeleteMeasurement(Guid measurementId, Guid checklistItemId)
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

		[Obsolete("Clients should provide checklistItemId")]
		[HttpDelete("Measurements/{measurementId}")]
		public ActionResult DeleteMeasurement(Guid measurementId)
		{
			var checklistItems = _context.ChecklistItems
				.Include(ci => ci.Measurements);


			var checklistItemInDb = checklistItems.Single(ci => ci.Measurements.Any(m => m.Id == measurementId));

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

			var cannedResponseResponse = Mapper.Map<CannedResponseResponse>(cannedResponse);

			return Ok(cannedResponseResponse);
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

		[Obsolete("Clients should provide checklistItemId")]
		[HttpDelete("CannedResponses/{cannedResponseId}")]
		public ActionResult DeleteCannedResponse(Guid cannedResponseId)
		{
			var checklistItemInDb = _context.ChecklistItems
				.Include(ci => ci.CannedResponses)
				.Single(ci => ci.CannedResponses.Any(m => m.Id == cannedResponseId));

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
