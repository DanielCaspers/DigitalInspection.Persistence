using System;
using System.Collections.Generic;
using DigitalInspectionNetCore21.Models.Inspections.Joins;

namespace DigitalInspectionNetCore21.Models.Inspections
{
	public class Inspection
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		public string WorkOrderId { get; set; }

		public virtual IList<ChecklistInspection> ChecklistInspections { get; set; } = new List<ChecklistInspection>();

		public virtual IList<ChecklistItemInspection> ChecklistItemInspections { get; set; } = new List<ChecklistItemInspection>();

		public virtual IList<InspectionItem> InspectionItems { get; set; } = new List<InspectionItem>();
	}
}