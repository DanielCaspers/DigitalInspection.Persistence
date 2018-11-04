using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DigitalInspectionNetCore21.Models.Inspections.Joins;

namespace DigitalInspectionNetCore21.Models.Inspections
{
	public class CannedResponse
	{
		[Required]
		public Guid Id { get; set; } = Guid.NewGuid();

		// Foreign key - Nullable allows individual deletion of response
		public Guid? ChecklistItemId { get; set; }

		// Navigation properties for model binding
		public virtual ChecklistItem ChecklistItem { get; set; }

		public virtual IList<InspectionItemCannedResponse> InspectionItemCannedResponses { get; set; } = new List<InspectionItemCannedResponse>();

		[Required(ErrorMessage = "Canned response is required")]
		[DisplayName("Canned Response *")]
		public string Response { get; set; }

		[DisplayName("URL")]
		public string Url { get; set; } = string.Empty;

		[DisplayName("Description")]
		public string Description { get; set; } = string.Empty;

		[DisplayName("Levels of Concern *")]
		[Required(ErrorMessage = "One or more levels of concern are required")]
		[Column("LevelsOfConcernInDb")]
		public IList<InspectionItemCondition> LevelsOfConcern { get; set; } = new List<InspectionItemCondition>();
	}
}