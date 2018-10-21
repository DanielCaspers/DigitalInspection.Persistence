using System;

namespace DigitalInspectionNetCore21.Models.Web.Inspections
{
	/// <summary>
	/// The measurement collected while performing an inspection
	/// </summary>
	public class InspectionMeasurementResponse
	{
		public Guid Id { get; set; }

		/// <summary>
		/// The id of the inspection item which the measurement corresponds to
		/// </summary>
		public Guid InspectionItemId { get; set; }

		/// <summary>
		/// The id of the measurement template describing this value collected
		/// </summary>
		public Guid MeasurementId { get; set; }

		/// <summary>
		/// The value collected
		/// </summary>
		/// <remarks>Null implies no value was collected</remarks>
		public int? Value { get; set; }
	}
}
