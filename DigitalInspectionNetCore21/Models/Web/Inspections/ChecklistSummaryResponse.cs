using System;

namespace DigitalInspectionNetCore21.Models.Web.Inspections
{
	public class ChecklistSummaryResponse
	{
		public Guid Id { get; set; }

		public int ChecklistItemsCount { get; set; }

		public string Name { get; set; }
	}
}
