using System;
using System.Collections.Generic;

namespace DigitalInspectionNetCore21.Models.Web.Inspections
{
	/// <summary>
	/// An inspection report
	/// </summary>
	public class InspectionSummaryResponse
	{
		public Guid Id { get; set; }

		public string WorkOrderId { get; set; }

		public IList<ChecklistItemSummaryResponse> Checklists { get; set; }
	}
}