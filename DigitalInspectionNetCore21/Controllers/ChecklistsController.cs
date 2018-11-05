using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using DigitalInspectionNetCore21.Controllers.Interfaces;
using DigitalInspectionNetCore21.Models.DbContexts;
using DigitalInspectionNetCore21.Models.Inspections;
using DigitalInspectionNetCore21.Models.Inspections.Joins;
using DigitalInspectionNetCore21.Models.Web;
using DigitalInspectionNetCore21.Models.Web.Checklists;
using DigitalInspectionNetCore21.Models.Web.Checklists.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Checklist = DigitalInspectionNetCore21.Models.Web.Checklists.Checklist;

namespace DigitalInspectionNetCore21.Controllers
{
	//[AuthorizeRoles(Roles.Admin)]
	[Route("[controller]")]
	public class ChecklistsController : BaseController, IChecklistsController
	{
		public ChecklistsController(ApplicationDbContext db) : base(db) { }

		/// <summary>
		/// Get all checklists in summary format
		/// </summary>
		/// <response code="200">Ok</response>
		[HttpGet("")]
		[ProducesResponseType(200)]
		public ActionResult<IEnumerable<ChecklistSummary>> GetAll()
		{
			var checklists = _context.Checklists
					.Include(c => c.ChecklistChecklistItems)
				.OrderBy(c => c.Name)
				.ToList();

			var checklistSummaryResponses = Mapper.Map<IEnumerable<ChecklistSummary>>(checklists);

			return Json(checklistSummaryResponses);
		}

		/// <summary>
		/// Get checklist, and all of its checklist items
		/// </summary>
		/// <param name="id"></param>
		/// <response code="200">Ok</response>
		/// <response code="404">Not found</response>
		[HttpGet("{id}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		public ActionResult<Checklist> GetById(Guid id)
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
				var checklistResponse = Mapper.Map<Checklist>(checklist);

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
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		public ActionResult<EditChecklistRequest> EditById(Guid id)
		{
			var checklist = _context.Checklists
				.Include(c => c.ChecklistChecklistItems)
				.SingleOrDefault(c => c.Id == id);

			if (checklist == null)
			{
				return NotFound();
			}

			IList<Models.Inspections.ChecklistItem> checklistItems = _context.ChecklistItems.OrderBy(c => c.Name).ToList();
			IList<bool> isChecklistItemSelected = checklistItems.Select(ci => checklist.ChecklistChecklistItems.Any(cci => cci.ChecklistItemId == ci.Id)).ToList();

			var viewModel = new EditChecklistRequest
			{
				Checklist = Mapper.Map<ChecklistSummary>(checklist),
				ChecklistItems = Mapper.Map<IList<Models.Web.Checklists.ChecklistItem>>(checklistItems),
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
		[ProducesResponseType(201)]
		public ActionResult<Checklist> Create([FromBody]AddChecklistRequest request)
		{
			var checklist = new Models.Inspections.Checklist
			{
				Name = request.Name,
				Id = Guid.NewGuid()
			};

			_context.Checklists.Add(checklist);
			_context.SaveChanges();

			var checklistResponse = Mapper.Map<Checklist>(checklist);

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
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		//https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-2.1
		public ActionResult Update(Guid id, [FromBody]EditChecklistRequest request)
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

				IList<Models.Inspections.ChecklistItem> selectedItems = new List<Models.Inspections.ChecklistItem>();
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
		[ProducesResponseType(204)]
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
