using System;

namespace DigitalInspectionNetCore21.Models.Web.Checklists
{
	/// <summary>
	/// A measurement to quantify the condition of an inspection item
	/// </summary>
	public class Measurement
	{
		public Guid Id { get; set; }

		/// <summary>
		/// The display name of the measurement to collect
		/// </summary>
		public string Label { get; set; }

		/// <summary>
		/// The minimum value of the measurement, inclusive
		/// </summary>
		public int MinValue { get; set; }

		/// <summary>
		/// The maximum value of the measurement, inclusive
		/// </summary>
		public int MaxValue { get; set; }

		/// <summary>
		/// The amount to increment/decrement by in the inspection by default
		/// </summary>
		public int StepSize { get; set; }

		/// <summary>
		/// The unit of measure being collected.
		/// </summary>
		/// <remarks>
		/// For additional precision, consider making your measurement units include the fraction by default.
		/// For example, if you want to measure tire tread depth, consider units of "32nds inch"
		/// </remarks>
		public string Unit { get; set; }
	}
}