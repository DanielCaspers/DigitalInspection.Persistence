using System;

namespace DigitalInspectionNetCore21.Models.Web.Inspections
{
	/// <summary>
	/// An image belonging to an inspection item performed on a vehicle. 
	/// </summary>
	public class InspectionImage
	{
		public Guid Id { get; set; }

		/// <summary>
		/// The title of the image file
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// The location of the image on the server
		/// </summary>
		public string ImageUrl { get; set; }

		/// <summary>
		/// The upload date of the image
		/// </summary>
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// The id of the associated inspection item
		/// </summary>
		public Guid InspectionItemId { get; set; }

		/// <summary>
		/// Determines if this image is visible in the customer's inspection report
		/// </summary>
		public bool IsVisibleToCustomer { get; set; } = true;
	}
}
