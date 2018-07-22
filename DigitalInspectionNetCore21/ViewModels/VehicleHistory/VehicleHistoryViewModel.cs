using System.Collections.Generic;
using DigitalInspectionNetCore21.Models.Orders;

namespace DigitalInspectionNetCore21.ViewModels.VehicleHistory
{
	public class VehicleHistoryViewModel
	{
		public IList<VehicleHistoryItem> VehicleHistory { get; set; } = new List<VehicleHistoryItem>();
	}
}