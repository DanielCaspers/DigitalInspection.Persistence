using System;
using System.Collections.Generic;

namespace DigitalInspectionNetCore21.Models.Web.Inspections
{
	/// <summary>
	/// A series of steps to perform to quanitfy the condition of a vehicle
	/// </summary>
	public class ChecklistResponse
	{
		public Guid Id { get; set; }

		/// <summary>
		/// The items which make up the checklist
		/// </summary>
		public IList<ChecklistItemResponse> ChecklistItems { get; set; }

		/// <summary>
		/// The name of the checklist
		/// </summary>
		public string Name { get; set; }
	}
}