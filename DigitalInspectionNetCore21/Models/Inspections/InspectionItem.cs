using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DigitalInspectionNetCore21.Models.Inspections.Joins;
using DigitalInspectionNetCore21.Models.Orders;

namespace DigitalInspectionNetCore21.Models.Inspections
{
	public class InspectionItem
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		public virtual Inspection Inspection { get; set; }

		public virtual ChecklistItem ChecklistItem { get; set; }

		public virtual IList<InspectionMeasurement> InspectionMeasurements { get; set; } = new List<InspectionMeasurement>();

		public string Note { get; set; }

		public RecommendedServiceSeverity Condition { get; set; }

		public virtual IList<InspectionItemCannedResponse> InspectionItemCannedResponses { get; set; } = new List<InspectionItemCannedResponse>();

		[NotMapped]
		public IList<Guid> SelectedCannedResponseIds { get; set; } = new List<Guid>();

		public virtual IList<InspectionImage> InspectionImages { get; set; } = new List<InspectionImage>();

		public bool IsCustomerConcern { get; set; }
	}
}