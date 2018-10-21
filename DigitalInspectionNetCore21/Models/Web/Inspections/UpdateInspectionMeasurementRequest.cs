using System;

namespace DigitalInspectionNetCore21.Models.Web.Inspections
{
	/// <summary>
	/// A key-value pair describing inspection measurements to update, and their values
	/// </summary>
	public class UpdateInspectionMeasurementRequest
	{
		/// <summary>
		/// ID of the inspection measurement being updated
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// The measurement collected
		/// </summary>
		/// <remarks>
		/// Null implies that the value was not collected
		/// </remarks>
		public int? Value { get; set; }
	}
}
