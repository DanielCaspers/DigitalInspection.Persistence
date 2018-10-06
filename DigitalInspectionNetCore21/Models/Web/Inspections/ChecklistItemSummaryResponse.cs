using System;
using System.Collections.Generic;

namespace DigitalInspectionNetCore21.Models.Web.Inspections
{
	public class ChecklistItemSummaryResponse
	{
		public Guid Id { get; set; }

		public IList<TagResponse> Tags { get; set; }

		public string Name { get; set; }

	}
}