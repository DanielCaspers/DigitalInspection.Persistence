using System;
using System.Collections.Generic;
using DigitalInspectionNetCore21.Models.Inspections;

namespace DigitalInspectionNetCore21.ViewModels.WorkOrders
{
	public class WorkOrderInspectionViewModel: WorkOrderDetailViewModel
	{
		public IList<Checklist> Checklists;

		public Guid InspectionId;
	}
}