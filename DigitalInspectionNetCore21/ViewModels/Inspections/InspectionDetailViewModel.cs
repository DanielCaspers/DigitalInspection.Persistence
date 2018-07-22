using System;
using DigitalInspectionNetCore21.Models.Inspections;
using DigitalInspectionNetCore21.Models.Orders;
using DigitalInspectionNetCore21.ViewModels.Base;
using DigitalInspectionNetCore21.ViewModels.TabContainers;
using DigitalInspectionNetCore21.ViewModels.VehicleHistory;

namespace DigitalInspectionNetCore21.ViewModels.Inspections
{
	public class InspectionDetailViewModel: BaseInspectionsViewModel
	{
		public WorkOrder WorkOrder { get; set; }

		public Checklist Checklist { get; set; }

		// Used for refreshing the view properly in the RedirectToAction() call in the controller after upload
		public Guid? FilteringTagId { get; set; }

		public Inspection Inspection { get; set; }

		public AddMeasurementViewModel AddMeasurementVM { get; set; }

		public AddInspectionItemNoteViewModel AddInspectionItemNoteVm { get; set; }

		public AddInspectionWorkOrderNoteViewModel AddInspectionWorkOrderNoteVm { get; set; }

		public UploadInspectionPhotosViewModel UploadInspectionPhotosVM { get; set; }

		public ViewInspectionPhotosViewModel ViewInspectionPhotosVM { get; set; }

		public VehicleHistoryViewModel VehicleHistoryVM { get; set; }

		public ConfirmDialogViewModel ConfirmInspectionCompleteViewModel { get; set; }

		public ScrollableTabContainerViewModel ScrollableTabContainerVM { get; set; }
	}
}