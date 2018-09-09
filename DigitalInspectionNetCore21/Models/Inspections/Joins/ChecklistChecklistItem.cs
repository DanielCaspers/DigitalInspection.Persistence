using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalInspectionNetCore21.Models.Inspections.Joins
{
	[Table("ChecklistChecklistItems")]
	public class ChecklistChecklistItem
    {
	    public Guid ChecklistId { get; set; }

	    public Checklist Checklist { get; set; }

	    public Guid ChecklistItemId { get; set; }

	    public ChecklistItem ChecklistItem { get; set; }
	}
}
