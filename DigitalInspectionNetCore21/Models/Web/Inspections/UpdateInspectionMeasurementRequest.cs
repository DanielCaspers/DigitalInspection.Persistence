using System;

namespace DigitalInspectionNetCore21.Models.Web.Inspections
{
	public class UpdateInspectionMeasurementRequest
	{
		public Guid Id { get; set; }

		public int? Value { get; set; }
	}
}
