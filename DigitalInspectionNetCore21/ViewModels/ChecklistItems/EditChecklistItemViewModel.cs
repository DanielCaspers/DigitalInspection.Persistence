using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DigitalInspectionNetCore21.Models.Orders;
using DigitalInspectionNetCore21.Models.Web.Inspections;
using DigitalInspectionNetCore21.ViewModels.Base;

namespace DigitalInspectionNetCore21.ViewModels.ChecklistItems
{
	public class EditChecklistItemViewModel: BaseChecklistsViewModel
	{
		public ChecklistItemResponse ChecklistItem { get; set; }

		[DisplayName("Tags *")]
		public IList<TagResponse> Tags { get; set; }

		[Required(ErrorMessage = "One or more tags are required")]
		public IEnumerable<Guid> SelectedTagIds { get; set; }

		public IEnumerable<RecommendedServiceSeverity> RecommendedServiceSeverities { get; set; } = new List<RecommendedServiceSeverity>()
		{
			RecommendedServiceSeverity.OK,
			RecommendedServiceSeverity.IMMEDIATE,
			RecommendedServiceSeverity.MODERATE,
			RecommendedServiceSeverity.SHOULD_WATCH,
			RecommendedServiceSeverity.MAINTENANCE,
			RecommendedServiceSeverity.NOTES,
			RecommendedServiceSeverity.NOT_APPLICABLE
		};
	}
}
