using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DigitalInspectionNetCore21.Models.Inspections.Joins;

namespace DigitalInspectionNetCore21.Models.Inspections
{
	public class Checklist
	{
		[Required]
		public Guid Id { get; set; }

		public virtual IList<ChecklistChecklistItem> ChecklistChecklistItems { get; set; } = new List<ChecklistChecklistItem>();

		[Required(ErrorMessage = "Checklist name is required")]
		[DisplayName("Checklist name *")]
		public string Name { get; set; }

		public virtual IList<ChecklistInspection> ChecklistInspections { get; set; } = new List<ChecklistInspection>();
	}
}