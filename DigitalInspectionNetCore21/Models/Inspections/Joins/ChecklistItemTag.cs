using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalInspectionNetCore21.Models.Inspections.Joins
{
	[Table("TagChecklistItems")]
	public class ChecklistItemTag
    {
		[Column("ChecklistItem_Id")]
	    public Guid ChecklistItemId { get; set; }

	    public ChecklistItem ChecklistItem { get; set; }

	    [Column("Tag_Id")]
		public Guid TagId { get; set; }

	    public Tag Tag { get; set; }
	}
}
