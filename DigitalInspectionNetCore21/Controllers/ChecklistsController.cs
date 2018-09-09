using DigitalInspectionNetCore21.Models;
using DigitalInspectionNetCore21.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using DigitalInspectionNetCore21.Models.DbContexts;
using DigitalInspectionNetCore21.Models.Inspections;
using DigitalInspectionNetCore21.Models.Inspections.Joins;
using DigitalInspectionNetCore21.Services.Core;
using DigitalInspectionNetCore21.ViewModels.Checklists;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DigitalInspectionNetCore21.Controllers
{
	//[AuthorizeRoles(Roles.Admin)]
	public class ChecklistsController : BaseController
	{
		private static readonly string IMAGE_SUBDIRECTORY = "Checklists";

		public ChecklistsController(ApplicationDbContext db) : base(db)
		{
			ResourceName = "Checklist";
		}

		private ManageChecklistMasterViewModel GetChecklistViewModel()
		{
			var checklists = _context.Checklists;

			return new ManageChecklistMasterViewModel
			{
				Checklists = checklists.OrderBy(c => c.Name).ToList(),
				AddChecklistVM = new AddChecklistViewModel
				{
					Name = ""
				}
			};
		}

		// GET: Checklists page and return response to index.cshtml
		public PartialViewResult Index()
		{
			return PartialView(GetChecklistViewModel());
		}

		// GET: Checklists_ChecklistList partial and return it to _ChecklistList.cshtml
		public PartialViewResult _ChecklistList()
		{
			return PartialView(GetChecklistViewModel());
		}

		//GET: Checklists/Edit/:id
		public PartialViewResult Edit(Guid id)
		{
			var checklist = _context.Checklists.SingleOrDefault(c => c.Id == id);
			IList<ChecklistItem> checklistItems = _context.ChecklistItems.OrderBy(c => c.Name).ToList();
			IList<bool> isChecklistItemSelected = new List<bool>();
			
			foreach(var checklistItem in checklistItems)
			{
				isChecklistItemSelected.Add(checklist.ChecklistChecklistItems.Select(joinItem => joinItem.ChecklistItem).Contains(checklistItem));
			}


			if (checklist == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(ResourceName));
			}
			else
			{
				var viewModel = new EditChecklistViewModel {
					Checklist = checklist,
					ChecklistItems = checklistItems,
					IsChecklistItemSelected = isChecklistItemSelected
				};
				return PartialView("_EditChecklist", viewModel);
			}
		}

		[HttpPost]
		//https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-2.1
		public ActionResult Update(Guid id, EditChecklistViewModel vm, IFormFile picture)
		{
			var checklistInDb = _context.Checklists.SingleOrDefault(c => c.Id == id);
			if(checklistInDb == null)
			{
				return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(ResourceName));
			}
			else
			{
				checklistInDb.Name = vm.Checklist.Name;

				IList<ChecklistItem> selectedItems = new List<ChecklistItem>();
				// Using plain for loop for parallel array data reference
				for(var i = 0; i < vm.IsChecklistItemSelected.Count; i++)
				{
					if (vm.IsChecklistItemSelected[i])
					{
						Guid selectedItemId = vm.ChecklistItems[i].Id;
						selectedItems.Add(_context.ChecklistItems.Single(ci => ci.Id == selectedItemId));
					}
				}
				foreach(var item in checklistInDb.ChecklistChecklistItems.Select(joinItem => joinItem.ChecklistItem))
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
				return RedirectToAction("Edit", new { id = checklistInDb.Id });
			}
		}

		[HttpPost]
		public ActionResult Create(AddChecklistViewModel list)
		{
			Checklist newList = new Checklist
			{
				Name = list.Name,
				Id = Guid.NewGuid(),
				Image = new Image()
			};

			_context.Checklists.Add(newList);
			_context.SaveChanges();

			return RedirectToAction("Index");
		}

		// POST: Checklist/Delete/5
		[HttpPost]
		public ActionResult Delete(Guid id)
		{
			try
			{
				var checklist = _context.Checklists.Find(id);

				if (checklist == null)
				{
					return PartialView("Toasts/_Toast", ToastService.ResourceNotFound(ResourceName));
				}

				_context.Checklists.Remove(checklist);
				_context.SaveChanges();
			}
			catch (Exception e)
			{
				return PartialView("Toasts/_Toast", ToastService.UnknownErrorOccurred(e));
			}
			return RedirectToAction("_ChecklistList");
		}

	}
}
