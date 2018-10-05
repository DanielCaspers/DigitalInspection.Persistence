using System;

namespace DigitalInspectionNetCore21.Models.Web.Inspections
{
	public class TagResponse
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public bool IsVisibleToCustomer { get; set; }

		public bool IsVisibleToEmployee { get; set; }
	}
}
