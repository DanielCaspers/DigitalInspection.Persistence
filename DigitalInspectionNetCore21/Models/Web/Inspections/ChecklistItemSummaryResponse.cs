using System;
using System.Collections.Generic;

namespace DigitalInspectionNetCore21.Models.Web.Inspections
{
	public class ChecklistItemSummaryResponse
	{
		public Guid Id { get; set; }

		// TODO At a minimum, I need ID, name and need to know it is currently selected
		public IList<TagResponse> Tags { get; set; }

		public string Name { get; set; }

	}
}