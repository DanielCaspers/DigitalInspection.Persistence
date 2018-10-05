using System;

namespace DigitalInspectionNetCore21.Models.Web.Inspections
{
	public class InspectionMeasurementResponse
	{
		public Guid Id { get; set; }

		public Guid InspectionItemId { get; set; }

		public Guid MeasurementId { get; set; }

		public int? Value { get; set; }
	}
}
