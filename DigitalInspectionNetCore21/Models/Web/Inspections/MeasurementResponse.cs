using System;

namespace DigitalInspectionNetCore21.Models.Web.Inspections
{
	public class MeasurementResponse
	{
		public Guid Id { get; set; }

		public string Label { get; set; }

		public int MinValue { get; set; }

		public int MaxValue { get; set; }

		public int StepSize { get; set; }

		public string Unit { get; set; }
	}
}