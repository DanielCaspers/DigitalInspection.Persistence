using System.Collections.Generic;
using DigitalInspectionNetCore21.Models.Inspections;
using DigitalInspectionNetCore21.ViewModels.Base;

namespace DigitalInspectionNetCore21.ViewModels.Checklists
{
	public class EditChecklistViewModel: BaseChecklistsViewModel
	{
		public Checklist Checklist { get; set; }
		
		public IList<ChecklistItem> ChecklistItems { get; set; }

		public IList<bool> IsChecklistItemSelected { get; set; }
	}
}