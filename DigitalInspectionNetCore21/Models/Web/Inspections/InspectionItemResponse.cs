using System;
using System.Collections.Generic;
using DigitalInspectionNetCore21.Models.Web.Orders;

namespace DigitalInspectionNetCore21.Models.Web.Inspections
{
	public class InspectionItemResponse
	{
		public Guid Id { get; set; }

		public Guid InspectionId { get; set; }

		public Guid ChecklistItemId { get; set; }

		public IList<InspectionMeasurementResponse> InspectionMeasurements { get; set; }

		public string Note { get; set; }

		public RecommendedServiceSeverity Condition { get; set; }

		public IList<Guid> CannedResponseIds { get; set; }

		public IList<Guid> SelectedCannedResponseIds { get; set; }

		public IList<InspectionImageResponse> InspectionImages { get; set; }

		public bool IsCustomerConcern { get; set; }
	}
}