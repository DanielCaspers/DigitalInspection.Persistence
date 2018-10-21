using System;
using System.Collections.Generic;
using DigitalInspectionNetCore21.Models.Orders;

namespace DigitalInspectionNetCore21.Models.Web.Inspections
{
	/// <summary>
	/// Used to describe a common problem that occurs on a checklist item
	/// </summary>
	/// <remarks>
	/// This is used in order to eliminate typing and cognitive load from those inspecting the vehicle.
	/// This also allows for rich, thoughtful descriptions of the nature of the problem to be described
	/// precisely every time they are marked down.
	/// </remarks>
	public class CannedResponseResponse
	{
		public Guid Id { get; set; }

		/// <summary>
		/// The shorthand description for the common problem associated unit of inspection.
		/// </summary>
		/// <remarks>This is to be used by employees when performing an inspection.</remarks>
		public string Response { get; set; }

		/// <summary>
		/// A url for linking to external resources describing the problem, or help content
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// A longer description of the problem
		/// </summary>
		/// <remarks>
		/// This is to be read by customers attempting
		///  to understand how this problem affects their vehicle.
		/// </remarks>
		public string Description { get; set; }

		/// <summary>
		/// A list of conditions, or severites, associated with this particular problem on the item being inspected.
		/// </summary>
		public IList<RecommendedServiceSeverity> LevelsOfConcern { get; set; }
	}
}