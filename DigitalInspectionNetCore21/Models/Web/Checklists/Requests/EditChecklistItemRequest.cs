using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DigitalInspectionNetCore21.Models.Web.Inspections;

namespace DigitalInspectionNetCore21.Models.Web.Checklists.Requests
{
	public class EditChecklistItemRequest
	{
		public ChecklistItem ChecklistItem { get; set; }

		[DisplayName("Tags *")]
		public IList<Tag> Tags { get; set; }

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
