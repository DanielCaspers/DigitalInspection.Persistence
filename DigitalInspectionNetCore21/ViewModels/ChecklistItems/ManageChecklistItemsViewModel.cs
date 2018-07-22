using System.Collections.Generic;
using DigitalInspectionNetCore21.Models.Inspections;
using DigitalInspectionNetCore21.ViewModels.Base;

namespace DigitalInspectionNetCore21.ViewModels.ChecklistItems
{
	public class ManageChecklistItemsViewModel: BaseChecklistsViewModel
	{
		public List<ChecklistItem> ChecklistItems { get; set; }
		public AddChecklistItemViewModel AddChecklistItemVM { get; set; }
	}
}