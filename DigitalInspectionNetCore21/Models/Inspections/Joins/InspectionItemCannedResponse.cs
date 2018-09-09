using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalInspectionNetCore21.Models.Inspections.Joins
{
	[Table("InspectionItemCannedResponses")]
	public class InspectionItemCannedResponse
    {
	    [Column("InspectionItem_Id")]
		public Guid InspectionItemId { get; set; }

		public InspectionItem InspectionItem { get; set; }

	    [Column("CannedResponse_Id")]
		public Guid CannedResponseId { get; set; }

		public CannedResponse CannedResponse { get; set; }
    }
}
