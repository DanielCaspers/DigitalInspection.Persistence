using System;
using System.Collections.Generic;

namespace DigitalInspectionNetCore21.Models.Web.Inspections
{
	public class InspectionItem
	{
		public Guid Id { get; set; }

		public Guid InspectionId { get; set; }

		public Guid ChecklistItemId { get; set; }

		public IList<InspectionMeasurement> InspectionMeasurements { get; set; }

		public string Note { get; set; }

		public Condition Condition { get; set; }

		public IList<Guid> CannedResponseIds { get; set; }

		public IList<Guid> SelectedCannedResponseIds { get; set; }

		public IList<InspectionImage> InspectionImages { get; set; }

		public bool IsCustomerConcern { get; set; }
	}
}