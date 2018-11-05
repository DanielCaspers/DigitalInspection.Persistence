using System;
using System.Collections.Generic;
using DigitalInspectionNetCore21.Models.Web.Checklists;

namespace DigitalInspectionNetCore21.Models.Web.Inspections
{
	/// <summary>
	/// An inspection report
	/// </summary>
	public class InspectionSummary
	{
		public Guid Id { get; set; }

		public string WorkOrderId { get; set; }

		public IList<ChecklistItemSummary> Checklists { get; set; }
	}
}