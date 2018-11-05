using System;

namespace DigitalInspectionNetCore21.Models.Web.Checklists
{
	public class ChecklistSummary
	{
		public Guid Id { get; set; }

		/// <summary>
		/// Number of checklist items in the checklist
		/// </summary>
		public int ChecklistItemsCount { get; set; }

		/// <summary>
		/// The name of the checklist
		/// </summary>
		public string Name { get; set; }
	}
}
