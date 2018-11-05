using System;
using System.Collections.Generic;

namespace DigitalInspectionNetCore21.Models.Web.Checklists
{
	/// <summary>
	/// A summary of a unit of work to be performed to determine the condition of a vehicle
	/// </summary>
	public class ChecklistItemSummary
	{
		public Guid Id { get; set; }

		/// <summary>
		/// The tags associated with this checklist item
		/// </summary>
		public IList<Tag> Tags { get; set; }

		/// <summary>
		/// The name of the unit of work
		/// </summary>
		public string Name { get; set; }
	}
}