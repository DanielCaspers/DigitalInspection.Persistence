using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using DigitalInspectionNetCore21.Models.DbContexts;
using DigitalInspectionNetCore21.Models.Inspections;
using DigitalInspectionNetCore21.Models.Inspections.Joins;
using DigitalInspectionNetCore21.Services.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DigitalInspectionNetCore21.Controllers.Interfaces;
using DigitalInspectionNetCore21.Models.Web.Checklists;
using DigitalInspectionNetCore21.Models.Web.Checklists.Requests;
using Measurement = DigitalInspectionNetCore21.Models.Web.Checklists.Measurement;
using Tag = DigitalInspectionNetCore21.Models.Web.Checklists.Tag;

namespace DigitalInspectionNetCore21.Controllers
{
	//[AuthorizeRoles(Roles.Admin)]
	[Route("[controller]")]
	public class ChecklistItemsController : BaseController, IChecklistItemsController
	{
		private readonly IChecklistItemRepository _checklistItemRepository;
		private readonly ITagRepository _tagRepository;

		public ChecklistItemsController(ApplicationDbContext db, IChecklistItemRepository checklistItemRepository, ITagRepository tagRepository) : base(db)
		{
			_checklistItemRepository = checklistItemRepository;
			_tagRepository = tagRepository;
		}

		/// <summary>
		/// Get all checklist items in summary format
		/// </summary>
		/// <response code="200">Ok</response>
		[HttpGet("")]
		[ProducesResponseType(200)]
		public ActionResult<IEnumerable<ChecklistItemSummary>> GetAll()
		{
			var checklistItems = _checklistItemRepository.GetAll().ToList();

			var checklistItemSummaryResponses = Mapper.Map<IEnumerable<ChecklistItemSummary>>(checklistItems);

			return Json(checklistItemSummaryResponses);
		}

		/// <summary>
		/// Get checklist item and all of its relationships
		/// </summary>
		/// <param name="id"></param>
		/// <response code="200">Ok</response>
		/// <response code="404">Not found</response>
		[HttpGet("{id}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		public ActionResult<Models.Web.Checklists.ChecklistItem> GetById(Guid id)
		{
			var checklistItem = _checklistItemRepository.GetById(id);

			if (checklistItem == null)
			{
				return NotFound();
			}
			else
			{
				var checklistItemResponse = Mapper.Map<Models.Web.Checklists.ChecklistItem>(checklistItem);
				return Json(checklistItemResponse);
			}
		}

		/// <summary>
		/// Get checklist item with related view model needs.
		/// </summary>
		/// <param name="id"></param>
		/// <response code="200">Ok</response>
		/// <response code="404">Not found</response>
		[Obsolete("Use only for Legacy .NET Framework App")]
		[HttpGet("{id}/Edit")]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		public ActionResult<EditChecklistItemRequest> EditById(Guid id)
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
				var viewModel = new EditChecklistItemRequest
				{
					ChecklistItem = Mapper.Map<Models.Web.Checklists.ChecklistItem>(checklistItem),
					Tags = Mapper.Map<IList<Tag>>(tags),
					SelectedTagIds = selectedTagIds
				};
				return Json(viewModel);
			}
		}

		/// <summary>
		/// Create a checklist item as a templated item for a future inspection
		/// </summary>
		/// <param name="request">Constraints to impose on the unit of inspection to be performed</param>
		/// <response code="201">Created</response>
		[HttpPost("")]
		[ProducesResponseType(201)]
		public ActionResult<Models.Web.Checklists.ChecklistItem> Create([FromBody]AddChecklistItemRequest request)
		{
			var checklistItem = new Models.Inspections.ChecklistItem()
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

			var checklistItemResponse = Mapper.Map<Models.Web.Checklists.ChecklistItem>(checklistItem);

			var createdUri = new Uri(HttpContext.Request.Path, UriKind.Relative);
			return Created(createdUri, checklistItemResponse);
		}

		/// <summary>
		///	Update a checklist item
		/// </summary>
		/// <param name="id"></param>
		/// <param name="request">Constraints to impose on the unit of inspection to be performed</param>
		/// <response code="204">No content</response>
		/// <response code="404">Checklist item not found</response>
		[HttpPut("{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		public ActionResult Update(Guid id, [FromBody]EditChecklistItemRequest request)
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

			foreach (var measurementInVm in request.ChecklistItem.Measurements)
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

			foreach (var cannedResponseInVm in request.ChecklistItem.CannedResponses)
			{
				var cannedResponseInDb = checklistItemInDb.CannedResponses.SingleOrDefault(cr => cr.Id == cannedResponseInVm.Id);
				if (cannedResponseInDb != null)
				{
					cannedResponseInDb.Response = cannedResponseInVm.Response;
					cannedResponseInDb.LevelsOfConcern =  (IList<InspectionItemCondition>) cannedResponseInVm.LevelsOfConcern.Select(condition => (InspectionItemCondition)condition.Value);
					cannedResponseInDb.Url = cannedResponseInVm.Url;
					cannedResponseInDb.Description = cannedResponseInVm.Description;
				}
			}

			checklistItemInDb.Name = request.ChecklistItem.Name;
			checklistItemInDb.ChecklistItemTags = _context.Tags
				.Where(t => request.SelectedTagIds.Contains(t.Id))
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

		/// <summary>
		/// Delete a checklist item
		/// </summary>
		/// <remarks>
		/// Checklist items cannot be deleted once they've been used in an inspection at the moment.
		/// This is currently done by design to prevent data loss in inspection reports.
		/// https://github.com/DanielCaspers/DigitalInspection/issues/74
		/// </remarks>
		/// <param name="id"></param>
		/// <response code="204">No content</response>
		[HttpDelete("{id}")]
		[ProducesResponseType(204)]
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

		/// <summary>
		/// Add a measurement to a checklist item
		/// </summary>
		/// <param name="id">The id of the checklist item to add the measurement to</param>
		/// <response code="200">Ok</response>
		/// <response code="404">Checklist item not found</response>
		[HttpPost("{id}/Measurements")]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		public ActionResult AddMeasurement(Guid id)
		{
			var checklistItem = _context.ChecklistItems.Find(id);

			if (checklistItem == null)
			{
				return NotFound();
			}

			var measurement = new Models.Inspections.Measurement();
			checklistItem.Measurements.Add(measurement);

			_context.SaveChanges();

			var measurementResponse = Mapper.Map<Measurement>(measurement);

			return Ok(measurementResponse);
		}

		/// <summary>
		/// Delete a measurement from a checklist item
		/// </summary>
		/// <param name="measurementId">The id of measurement to delete</param>
		/// <param name="checklistItemId">The id of checklist item which the measurement belongs to</param>
		/// <response code="204">No content</response>
		[HttpDelete("{checklistItemId}/Measurements/{measurementId}")]
		[ProducesResponseType(204)]
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

		/// <summary>
		/// Delete a measurement from a checklist item
		/// </summary>
		/// <param name="measurementId">The id of measurement to delete</param>
		/// <response code="204">No content</response>
		[Obsolete("Clients should provide checklistItemId")]
		[HttpDelete("Measurements/{measurementId}")]
		[ProducesResponseType(204)]
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

		/// <summary>
		/// Add a canned repsonse to a checklist item
		/// </summary>
		/// <param name="id">The id of the checklist item to add the canned response to</param>
		/// <response code="200">Ok</response>
		/// <response code="404">Checklist item not found</response>
		[HttpPost("{id}/CannedResponses")]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		public ActionResult AddCannedResponse(Guid id)
		{
			var checklistItemInDb = _context.ChecklistItems.Find(id);

			if (checklistItemInDb == null)
			{
				return NotFound();
			}

			var cannedResponse = new Models.Inspections.CannedResponse()
			{
				Response = "A new response"
			};
			checklistItemInDb.CannedResponses.Add(cannedResponse);

			_context.SaveChanges();

			var cannedResponseResponse = Mapper.Map<Models.Web.Checklists.CannedResponse>(cannedResponse);

			return Ok(cannedResponseResponse);
		}

		/// <summary>
		/// Delete a canned response from a checklist item
		/// </summary>
		/// <param name="cannedResponseId">The id of canned response to delete</param>
		/// <param name="checklistItemId">The id of checklist item which the canned response belongs to</param>
		/// <response code="204">No content</response>
		[HttpDelete("{checklistItemId}/CannedResponses/{cannedResponseId}")]
		[ProducesResponseType(204)]
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

		/// <summary>
		/// Delete a canned response from a checklist item
		/// </summary>
		/// <param name="cannedResponseId">The id of canned response to delete</param>
		/// <response code="204">No content</response>
		[Obsolete("Clients should provide checklistItemId")]
		[HttpDelete("CannedResponses/{cannedResponseId}")]
		[ProducesResponseType(204)]
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
