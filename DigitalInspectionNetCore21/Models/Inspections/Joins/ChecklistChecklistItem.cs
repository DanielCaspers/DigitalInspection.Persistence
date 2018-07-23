using System;

namespace DigitalInspectionNetCore21.Models.Inspections.Joins
{
    public class ChecklistChecklistItem
    {
	    public Guid ChecklistId { get; set; }

	    public Checklist Checklist { get; set; }

	    public Guid ChecklistItemId { get; set; }

	    public ChecklistItem ChecklistItem { get; set; }
	}
}
