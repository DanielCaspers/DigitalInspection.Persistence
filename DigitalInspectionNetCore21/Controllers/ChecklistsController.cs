using System;
using System.Collections.Generic;
using System.Linq;
using DigitalInspectionNetCore21.Models.DbContexts;
using DigitalInspectionNetCore21.Models.Inspections;
using DigitalInspectionNetCore21.Models.Inspections.Joins;
using DigitalInspectionNetCore21.ViewModels.Checklists;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalInspectionNetCore21.Controllers
{
	//[AuthorizeRoles(Roles.Admin)]
	[Route("[controller]")]
	public class ChecklistsController : BaseController, IRepositoryController<Checklist, AddChecklistViewModel, EditChecklistViewModel>
	{
		public ChecklistsController(ApplicationDbContext db) : base(db) { }

		[HttpGet("")]
		public ActionResult<IEnumerable<Checklist>> GetAll()
		{
			var checklistItems = _context.Checklists
					.Include(c => c.ChecklistChecklistItems)
				.OrderBy(c => c.Name)
				.ToList();

			return Json(checklistItems);
		}

		[HttpGet("{id}")]
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
				return Json(checklist);
			}
		}
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
				Checklist = checklist,
				ChecklistItems = checklistItems,
				IsChecklistItemSelected = isChecklistItemSelected
			};
			return Json(viewModel);
		}

		[HttpPost("")]
		public ActionResult<Checklist> Create([FromBody]AddChecklistViewModel request)
		{
			var checklist = new Checklist
			{
				Name = request.Name,
				Id = Guid.NewGuid()
			};

			_context.Checklists.Add(checklist);
			_context.SaveChanges();

			var createdUri = new Uri(HttpContext.Request.Path, UriKind.Relative);
			return Created(createdUri, checklist);
		}

		[HttpPut("{id}")]
		//https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-2.1
		public ActionResult<Checklist> Update(Guid id, [FromBody]EditChecklistViewModel vm)
		{
			var checklistInDb = _context.Checklists
				.Include(ci => ci.ChecklistChecklistItems)
				.SingleOrDefault(c => c.Id == id);
			if (checklistInDb == null)
			{
				return NotFound();
			}
			else
			{
				checklistInDb.Name = vm.Checklist.Name;

				IList<ChecklistItem> selectedItems = new List<ChecklistItem>();
				// Using plain for loop for parallel array data reference
				for (var i = 0; i < vm.IsChecklistItemSelected.Count; i++)
				{
					if (vm.IsChecklistItemSelected[i])
					{
						Guid selectedItemId = vm.ChecklistItems[i].Id;
						selectedItems.Add(_context.ChecklistItems.Single(ci => ci.Id == selectedItemId));
					}
				}
				foreach (var item in checklistInDb.ChecklistChecklistItems.Select(joinItem => joinItem.ChecklistItem))
				{
					_context.ChecklistItems.Attach(item);
				}
				// FIXME DJC EF Many2Many - This could break the relationship
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
