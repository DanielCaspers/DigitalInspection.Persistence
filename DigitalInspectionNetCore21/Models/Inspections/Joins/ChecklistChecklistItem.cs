using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalInspectionNetCore21.Models.Inspections.Joins
{
	[Table("ChecklistChecklistItems")]
	public class ChecklistChecklistItem
    {
	    [Column("Checklist_Id")]
		public Guid ChecklistId { get; set; }

	    public Checklist Checklist { get; set; }

	    [Column("ChecklistItem_Id")]
		public Guid ChecklistItemId { get; set; }

	    public ChecklistItem ChecklistItem { get; set; }
	}
}
