using System;
using System.Collections.Generic;

namespace DigitalInspectionNetCore21.Models.Web.Checklists
{
	/// <summary>
	/// A unit of work to be performed to determine the condition of a vehicle
	/// </summary>
	public class ChecklistItem
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
		public IList<Tag> Tags { get; set; }

		public IList<CannedResponse> CannedResponses { get; set; }

		public IList<Measurement> Measurements { get; set; }
	}
}