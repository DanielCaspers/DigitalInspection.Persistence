using System;

namespace DigitalInspectionNetCore21.Models.Web.Inspections
{
	public class InspectionImageResponse
	{
		public Guid Id { get; set; }

		public string Title { get; set; }

		public string ImageUrl { get; set; }

		public DateTime CreatedDate { get; set; }

		public Guid InspectionItemId { get; set; }

		// Determines if visible in the customer's inspection report
		public bool IsVisibleToCustomer { get; set; } = true;
	}
}
