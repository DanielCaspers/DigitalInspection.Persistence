using System;
using System.Collections.Generic;

namespace DigitalInspectionNetCore21.Models.Web.Inspections
{
	public class ChecklistItemResponse
	{
		public Guid Id { get; set; }

		public IList<Guid> ChecklistIds { get; set; }

		public IList<Guid> InspectionIds { get; set; }

		public IList<Guid> InspectionItemIds { get; set; }

		public string Name { get; set; }

		public IList<TagResponse> Tags { get; set; }

		public IList<CannedResponseResponse> CannedResponses { get; set; }

		public IList<MeasurementResponse> Measurements { get; set; }
	}
}