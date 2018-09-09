using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalInspectionNetCore21.Models.Inspections.Joins
{
	[Table("InspectionItemCannedResponses")]
	public class InspectionItemCannedResponse
    {
		public Guid InspectionItemId { get; set; }

		public InspectionItem InsepctionItem { get; set; }

		public Guid CannedResponseId { get; set; }

		public CannedResponse CannedResponse { get; set; }
    }
}
