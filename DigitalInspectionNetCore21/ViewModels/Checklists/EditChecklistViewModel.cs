using System.Collections.Generic;
using DigitalInspectionNetCore21.Models.Web.Inspections;

namespace DigitalInspectionNetCore21.ViewModels.Checklists
{
	public class EditChecklistViewModel
	{
		public ChecklistSummaryResponse Checklist { get; set; }
		
		public IList<ChecklistItemResponse> ChecklistItems { get; set; }

		public IList<bool> IsChecklistItemSelected { get; set; }
	}
}