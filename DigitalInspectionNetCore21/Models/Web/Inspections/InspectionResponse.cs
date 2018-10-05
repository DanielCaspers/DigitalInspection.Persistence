using System;
using System.Collections.Generic;

namespace DigitalInspectionNetCore21.Models.Web.Inspections
{
	public class InspectionResponse
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		public string WorkOrderId { get; set; }

		// Should have top level props like checklist id and name
		public IList<ChecklistResponse> Checklists { get; set; } = new List<ChecklistResponse>();

		// Should have all checklist items
		public IList<ChecklistItemResponse> ChecklistItems { get; set; } = new List<ChecklistItemResponse>();

		public IList<InspectionItemResponse> InspectionItems { get; set; } = new List<InspectionItemResponse>();
	}
}