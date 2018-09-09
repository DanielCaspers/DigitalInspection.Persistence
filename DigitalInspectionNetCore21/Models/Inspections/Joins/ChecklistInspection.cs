using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalInspectionNetCore21.Models.Inspections.Joins
{
	[Table("ChecklistInspections")]
	public class ChecklistInspection
    {
	    [Column("Checklist_Id")]
		public Guid ChecklistId { get; set; }

	    public Checklist Checklist { get; set; }

	    [Column("Inspection_Id")]
		public Guid InspectionId { get; set; }

	    public Inspection Inspection { get; set; }
	}
}
