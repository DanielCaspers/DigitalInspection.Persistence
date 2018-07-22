using System.Collections.Generic;
using DigitalInspectionNetCore21.Models.Orders;
using DigitalInspectionNetCore21.ViewModels.Base;

namespace DigitalInspectionNetCore21.ViewModels.WorkOrders
{
	public class WorkOrderMasterViewModel: BaseWorkOrdersViewModel
	{
		public IList<WorkOrder> WorkOrders { get; set; }
	}
}