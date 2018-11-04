using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DigitalInspectionNetCore21.Models.Web.Inspections;

namespace DigitalInspectionNetCore21.ViewModels.ChecklistItems
{
	public class EditChecklistItemViewModel
	{
		public ChecklistItemResponse ChecklistItem { get; set; }

		[DisplayName("Tags *")]
		public IList<TagResponse> Tags { get; set; }

		[Required(ErrorMessage = "One or more tags are required")]
		public IEnumerable<Guid> SelectedTagIds { get; set; }

		public IEnumerable<Condition> RecommendedServiceSeverities { get; set; } = new List<Condition>()
		{
			Condition.Ok,
			Condition.Immediate,
			Condition.Moderate,
			Condition.ShouldWatch,
			Condition.Maintenance,
			Condition.Notes,
			Condition.NotApplicable
		};
	}
}
