using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalInspectionNetCore21.Models.Inspections.Joins
{
	[Table("ChecklistInspections")]
	public class ChecklistInspection
    {
	    public Guid ChecklistId { get; set; }

	    public Checklist Checklist { get; set; }

	    public Guid InspectionId { get; set; }

	    public Inspection Inspection { get; set; }
	}
}
