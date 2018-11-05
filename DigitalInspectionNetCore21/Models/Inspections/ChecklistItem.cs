using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DigitalInspectionNetCore21.Models.Inspections.Joins;

namespace DigitalInspectionNetCore21.Models.Inspections
{
	public class ChecklistItem
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		public virtual IList<ChecklistChecklistItem> ChecklistChecklistItems { get; set; } = new List<ChecklistChecklistItem>();

		// FIXME DJC This is an empty table, and EF gave no errors when commenting many-many relationship, so perhaps the table can be deleted?
		//public virtual IList<Inspection> Inspections { get; set; } = new List<Inspection>();
		public virtual IList<ChecklistItemInspection> ChecklistItemInspections { get; set; } = new List<ChecklistItemInspection>();

		public virtual IList<InspectionItem> InspectionItems { get; set; } = new List<InspectionItem>();

		//[Required(ErrorMessage = "Checklist item name is required")]
		//[DisplayName("Checklist item name *")]
		public string Name { get; set; }

		[Required]
		public virtual IList<ChecklistItemTag> ChecklistItemTags { get; set; }

		// Virtual lazy loads and makes EF less dumb 
		// http://stackoverflow.com/a/9246932/2831961
		[Required] // TODO: REVISIT REQUIRED
		public virtual IList<CannedResponse> CannedResponses { get; set; } = new List<CannedResponse>();

		[Required]
		public virtual IList<Measurement> Measurements { get; set; } = new List<Measurement>();
	}
}