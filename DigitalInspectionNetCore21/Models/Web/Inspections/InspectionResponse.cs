using System;
using System.Collections.Generic;
using DigitalInspectionNetCore21.Models.Web.Checklists;

namespace DigitalInspectionNetCore21.Models.Web.Inspections
{
	public class InspectionResponse
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		public string WorkOrderId { get; set; }

		// Should have top level props like checklist id and name
		public IList<Checklist> Checklists { get; set; } = new List<Checklist>();

		// Should have all checklist items
		public IList<ChecklistItem> ChecklistItems { get; set; } = new List<ChecklistItem>();

		public IList<InspectionItem> InspectionItems { get; set; } = new List<InspectionItem>();
	}
}