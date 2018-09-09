using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalInspectionNetCore21.Models.Inspections.Joins
{
	[Table("ChecklistItemInspections")]
	public class ChecklistItemInspection
    {
	    public Guid ChecklistItemId { get; set; }

	    public ChecklistItem ChecklistItem { get; set; }

	    public Guid InspectionId { get; set; }

	    public Inspection Inspection { get; set; }
	}
}
