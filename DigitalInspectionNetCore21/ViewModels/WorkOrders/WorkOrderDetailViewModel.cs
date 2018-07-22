using DigitalInspectionNetCore21.Models.Orders;
using DigitalInspectionNetCore21.ViewModels.Base;
using DigitalInspectionNetCore21.ViewModels.Inspections;
using DigitalInspectionNetCore21.ViewModels.TabContainers;
using DigitalInspectionNetCore21.ViewModels.VehicleHistory;

namespace DigitalInspectionNetCore21.ViewModels.WorkOrders
{
	public class WorkOrderDetailViewModel: BaseWorkOrdersViewModel
	{
		public WorkOrder WorkOrder { get; set; }

		public VehicleHistoryViewModel VehicleHistoryVM { get; set; }

		public AddInspectionWorkOrderNoteViewModel AddInspectionWorkOrderNoteVm { get; set; }

		public bool CanEdit { get; set; } = false;

		public TabContainerViewModel TabViewModel { get; set; }
	}
}