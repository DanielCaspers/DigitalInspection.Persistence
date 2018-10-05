using System;
using System.Collections.Generic;

namespace DigitalInspectionNetCore21.Models.Web.Inspections
{
	public class ChecklistResponse
	{
		public Guid Id { get; set; }

		public IList<ChecklistItemResponse> ChecklistItems { get; set; }

		public string Name { get; set; }
	}
}