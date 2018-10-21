using System;

namespace DigitalInspectionNetCore21.Models.Web.Inspections
{
	public class ChecklistSummaryResponse
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
