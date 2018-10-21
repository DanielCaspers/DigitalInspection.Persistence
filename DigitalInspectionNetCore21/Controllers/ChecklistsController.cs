using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using DigitalInspectionNetCore21.Models.DbContexts;
using DigitalInspectionNetCore21.Models.Inspections;
using DigitalInspectionNetCore21.Models.Inspections.Joins;
using DigitalInspectionNetCore21.Models.Web.Inspections;
using DigitalInspectionNetCore21.ViewModels.Checklists;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalInspectionNetCore21.Controllers
{
	//[AuthorizeRoles(Roles.Admin)]
	[Route("[controller]")]
	public class ChecklistsController : BaseController
	{
		public ChecklistsController(ApplicationDbContext db) : base(db) { }

		/// <summary>
		/// Get all checklists in summary format
		/// </summary>
		/// <response code="200">Ok</response>
		[HttpGet("")]
		// TODO DJC Update this repository pattern to be an more forgiving to different model types, since I'm using summary models here
		public ActionResult<IEnumerable<ChecklistSummaryResponse>> GetAll()
		{
			var checklists = _context.Checklists
					.Include(c => c.ChecklistChecklistItems)
				.OrderBy(c => c.Name)
				.ToList();

			var checklistSummaryResponses = Mapper.Map<IEnumerable<ChecklistSummaryResponse>>(checklists);

			return Json(checklistSummaryResponses);
		}

		/// <summary>
		/// Get checklist, and all of its checklist items
		/// </summary>
		/// <param name="id"></param>
		/// <response code="200">Ok</response>
		/// <response code="404">Not found</response>
		[HttpGet("{id}")]
		public ActionResult<ChecklistResponse> GetById(Guid id)
		{
			var checklist = _context.Checklists
				.Include(c => c.ChecklistChecklistItems)
				.SingleOrDefault(c => c.Id == id);

			if (checklist == null)
			{
				return NotFound();
			}
			else
			{
				var checklistResponse = Mapper.Map<ChecklistResponse>(checklist);

				return Json(checklistResponse);
			}
		}

		/// <summary>
		/// Get checklist with related view model needs
		/// </summary>
		/// <param name="id"></param>
		/// <response code="200">Ok</response>
		/// <response code="404">Not found</response>
		[Obsolete("Use only for Legacy .NET Framework App")]
		[HttpGet("{id}/Edit")]
		public ActionResult<EditChecklistViewModel> EditById(Guid id)
		{
			var checklist = _context.Checklists
				.Include(c => c.ChecklistChecklistItems)
				.SingleOrDefault(c => c.Id == id);

			if (checklist == null)
			{
				return NotFound();
			}

			IList<ChecklistItem> checklistItems = _context.ChecklistItems.OrderBy(c => c.Name).ToList();
			IList<bool> isChecklistItemSelected = checklistItems.Select(ci => checklist.ChecklistChecklistItems.Any(cci => cci.ChecklistItemId == ci.Id)).ToList();

			var viewModel = new EditChecklistViewModel
			{
				Checklist = Mapper.Map<ChecklistSummaryResponse>(checklist),
				ChecklistItems = Mapper.Map<IList<ChecklistItemResponse>>(checklistItems),
				IsChecklistItemSelected = isChecklistItemSelected
			};
			return Json(viewModel);
		}

		/// <summary>
		/// Create a checklist as a template for a future inspection
		/// </summary>
		/// <param name="request">
		/// Constraints to impose on the inspection to be performed,
		///  such as which checklist items must be performed
		/// </param>
		/// <response code="201">Created</response>
		[HttpPost("")]
		public ActionResult<ChecklistResponse> Create([FromBody]AddChecklistViewModel request)
		{
			var checklist = new Checklist
			{
				Name = request.Name,
				Id = Guid.NewGuid()
			};

			_context.Checklists.Add(checklist);
			_context.SaveChanges();

			var checklistResponse = Mapper.Map<ChecklistResponse>(checklist);

			var createdUri = new Uri(HttpContext.Request.Path, UriKind.Relative);
			return Created(createdUri, checklistResponse);
		}

		/// <summary>
		///	Update a checklist
		/// </summary>
		/// <param name="id"></param>
		/// <param name="request">Constraints to impose on the inspection to be performed</param>
		/// <response code="204">No content</response>
		/// <response code="404">Checklist item not found</response>
		[HttpPut("{id}")]
		//https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-2.1
		public ActionResult Update(Guid id, [FromBody]EditChecklistViewModel request)
		{
			var checklistInDb = _context.Checklists
				.Include(ci => ci.ChecklistChecklistItems)
					.ThenInclude(cci => cci.ChecklistItem)
				.SingleOrDefault(c => c.Id == id);
			if (checklistInDb == null)
			{
				return NotFound();
			}
			else
			{
				checklistInDb.Name = request.Checklist.Name;

				IList<ChecklistItem> selectedItems = new List<ChecklistItem>();
				// Using plain for loop for parallel array data reference
				for (var i = 0; i < request.IsChecklistItemSelected.Count; i++)
				{
					if (request.IsChecklistItemSelected[i])
					{
						Guid selectedItemId = request.ChecklistItems[i].Id;
						selectedItems.Add(_context.ChecklistItems.Single(ci => ci.Id == selectedItemId));
					}
				}
				foreach (var item in checklistInDb.ChecklistChecklistItems.Select(joinItem => joinItem.ChecklistItem))
				{
					_context.ChecklistItems.Attach(item);
				}
				checklistInDb.ChecklistChecklistItems = selectedItems.Select(ci => new ChecklistChecklistItem
				{
					Checklist = checklistInDb,
					ChecklistId = checklistInDb.Id,
					ChecklistItem = ci,
					ChecklistItemId = ci.Id
				}).ToList();

				_context.SaveChanges();
				return NoContent();
			}
		}

		/// <summary>
		/// Delete a checklist
		/// </summary>
		/// <remarks>
		/// Deleting a checklist leaves all performed inspections, as well as their inspection items intact.
		/// This is by design, so as to allow templates for work to be performed to change fluidly, but the underlying
		/// units, once performed, should stick around to prevent loss of inspection reporting capability.
		/// https://github.com/DanielCaspers/DigitalInspection/issues/74
		/// </remarks>
		/// <param name="id"></param>
		/// <response code="204">No content</response>
		[HttpDelete("{id}")]
		public NoContentResult Delete(Guid id)
		{
			var checklist = _context.Checklists.Find(id);

			if (checklist != null)
			{
				_context.Checklists.Remove(checklist);
				_context.SaveChanges();
			}

			return NoContent();
		}
	}
}
