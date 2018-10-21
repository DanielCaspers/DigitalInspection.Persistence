using System;
using System.Collections.Generic;

namespace DigitalInspectionNetCore21.Models.Web.Inspections
{
	/// <summary>
	/// A unit of work to be performed to determine the condition of a vehicle
	/// </summary>
	public class ChecklistItemResponse
	{
		public Guid Id { get; set; }

		public IList<Guid> ChecklistIds { get; set; }

		public IList<Guid> InspectionIds { get; set; }

		public IList<Guid> InspectionItemIds { get; set; }

		/// <summary>
		/// The name of the unit of work
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The tags associated with this checklist item
		/// </summary>
		public IList<TagResponse> Tags { get; set; }

		public IList<CannedResponseResponse> CannedResponses { get; set; }

		public IList<MeasurementResponse> Measurements { get; set; }
	}
}