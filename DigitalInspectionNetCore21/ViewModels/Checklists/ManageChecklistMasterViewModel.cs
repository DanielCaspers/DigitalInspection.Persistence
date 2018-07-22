using System.Collections.Generic;
using DigitalInspectionNetCore21.Models.Inspections;
using DigitalInspectionNetCore21.ViewModels.Base;

namespace DigitalInspectionNetCore21.ViewModels.Checklists
{
	public class ManageChecklistMasterViewModel: BaseChecklistsViewModel
	{
		public List<Checklist> Checklists { get; set; }
		public AddChecklistViewModel AddChecklistVM { get; set; }
	}
}