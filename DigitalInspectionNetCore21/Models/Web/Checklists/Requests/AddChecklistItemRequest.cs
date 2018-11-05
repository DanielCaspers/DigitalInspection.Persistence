using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DigitalInspectionNetCore21.Models.Web.Checklists.Requests
{
	public class AddChecklistItemRequest
	{
		[Required(ErrorMessage = "Checklist item name is required")]
		[DisplayName("Checklist item name *")]
		public string Name { get; set; }


		[DisplayName("Tags *")]
		[Required(ErrorMessage = "One or more tags are required")]
		public IList<Tag> Tags { get; set; }

		public IList<Measurement> Measurements { get; set; }

		// TODO Clean this up post .NET Core port
		public IList<Guid> TagIds { get; set; } = new List<Guid>();
	}
}