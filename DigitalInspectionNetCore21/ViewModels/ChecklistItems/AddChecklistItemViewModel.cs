using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DigitalInspectionNetCore21.Models.Inspections;

namespace DigitalInspectionNetCore21.ViewModels.ChecklistItems
{
	public class AddChecklistItemViewModel
	{
		[Required(ErrorMessage = "Checklist item name is required")]
		[DisplayName("Checklist item name *")]
		public string Name { get; set; }


		[DisplayName("Tags *")]
		[Required(ErrorMessage = "One or more tags are required")]
		public IList<Tag> Tags { get; set; }

		public IList<Measurement> Measurements { get; set; } = new List<Measurement>();

		// TODO Clean this up post .NET Core port
		public IList<Guid> TagIds { get; set; } = new List<Guid>();
	}
}