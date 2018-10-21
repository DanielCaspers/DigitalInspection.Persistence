using System;

namespace DigitalInspectionNetCore21.Models.Web.Inspections
{
	/// <summary>
	/// Tags are used to 
	/// - Group what customers see down by subsystem or feature in order for them to better identify
	/// the scope and size of the problems with their vehicle.
	/// - Allow employees to filter by a logical grouping so that they can order their inspections 
	/// by the most relevant sections to perform at any given time.
	/// </summary>
	public class TagResponse
	{
		public Guid Id { get; set; }

		/// <summary>
		/// The display name of the tag. 
		/// </summary>
		/// <remarks>
		/// This is both employee and customer facing.
		/// For example, this can be used to divide an inspection into logical
		/// subsystems or subsets (e.g. Exterior, Interior, Tires & Brakes, Drivetrain, etc)
		/// </remarks>
		public string Name { get; set; }

		/// <summary>
		/// Whether or not any the results of performing any associated checklist items
		/// should be included in the customer's inspection report
		/// </summary>
		public bool IsVisibleToCustomer { get; set; }

		/// <summary>
		/// Whether or not the grouping should be visible to employees
		/// </summary>
		/// <remarks>
		/// For example, you may find that certain groupings work well with explaining
		/// the results to a customer, but provide a suboptimal flow for those performing the inspection.
		/// Creating separate tags for customers and employees can sometimes be ideal depending upon use case.
		/// </remarks>
		public bool IsVisibleToEmployee { get; set; }
	}
}
