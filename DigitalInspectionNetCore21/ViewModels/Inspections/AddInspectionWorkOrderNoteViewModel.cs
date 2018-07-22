using System.ComponentModel;

namespace DigitalInspectionNetCore21.ViewModels.Inspections
{
	public class AddInspectionWorkOrderNoteViewModel
	{
		public string WorkOrderId { get; set; }

		[DisplayName("Note")]
		public string Note { get; set; }
	}
}