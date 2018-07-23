using System;

namespace DigitalInspectionNetCore21.Models.Inspections.Joins
{
    public class InspectionItemCannedResponse
    {
		public Guid InspectionItemId { get; set; }

		public InspectionItem InsepctionItem { get; set; }

		public Guid CannedResponseId { get; set; }

		public CannedResponse CannedResponse { get; set; }
    }
}
