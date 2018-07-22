using System.ComponentModel;
using DigitalInspectionNetCore21.Models.Inspections;

namespace DigitalInspectionNetCore21.ViewModels.Inspections
{
	public class AddInspectionItemNoteViewModel
	{
		public ChecklistItem ChecklistItem { get; set; }

		public InspectionItem InspectionItem { get; set; }

		[DisplayName("Note")]
		public string Note { get; set; }
	}
}