using System.Collections.Generic;

namespace DigitalInspectionNetCore21.Models.Web.Checklists.Requests
{
	public class EditChecklistRequest
	{
		public ChecklistSummary Checklist { get; set; }
		
		public IList<ChecklistItem> ChecklistItems { get; set; }

		public IList<bool> IsChecklistItemSelected { get; set; }
	}
}