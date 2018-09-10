using System;
using System.Linq;
using DigitalInspectionNetCore21.Models.DbContexts;
using DigitalInspectionNetCore21.Models.Inspections;
using DigitalInspectionNetCore21.Services.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DigitalInspectionNetCore21.Services.Core
{
	/// <inheritdoc cref="IChecklistItemRepository"/>
	public class ChecklistItemRepository : IChecklistItemRepository
	{
		private readonly ApplicationDbContext _context;

		public ChecklistItemRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public IQueryable<ChecklistItem> GetAll()
		{
			return _context.ChecklistItems.OrderBy(ci => ci.Name);
		}

		public ChecklistItem GetById(Guid id)
		{
			var checklistItem =  _context.ChecklistItems
				.Include(ci => ci.ChecklistItemTags)
					.ThenInclude(cit => cit.Tag)
				.Include(ci => ci.CannedResponses)
				.Include(ci => ci.Measurements)
				.SingleOrDefault(c => c.Id == id);

			if (checklistItem != null)
			{
				checklistItem.Measurements = checklistItem.Measurements.OrderBy(m => m.Label).ToList();
				checklistItem.CannedResponses = checklistItem.CannedResponses.OrderBy(c => c.Response).ToList();
			}

			return checklistItem;
		}
	}
}
