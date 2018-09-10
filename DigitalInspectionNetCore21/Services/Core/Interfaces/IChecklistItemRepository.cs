using System;
using System.Linq;
using DigitalInspectionNetCore21.Models.Inspections;

namespace DigitalInspectionNetCore21.Services.Core.Interfaces
{
	/// <summary>
	/// Used for facading query logic for checklist items
	/// </summary>
	public interface IChecklistItemRepository
	{
		/// <summary>
		/// Gets all checklist items, sorted by name
		/// </summary>
		IQueryable<ChecklistItem> GetAll();

		/// <summary>
		/// Gets checklist with fields sorted by name
		/// </summary>
		/// <remarks>
		/// Includes relational properties
		/// </remarks>
		/// <param name="id"></param>
		/// <returns></returns>
		ChecklistItem GetById(Guid id);
	}
}