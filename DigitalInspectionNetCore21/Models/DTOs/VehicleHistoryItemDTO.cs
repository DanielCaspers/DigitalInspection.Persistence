
using System.Collections.Generic;

namespace DigitalInspectionNetCore21.Models.DTOs
{
	public class VehicleHistoryItemDTO
	{
		public string orderID { get; set; }

		public long? completionDate { get; set; }

		public int? vehicleOdometer { get; set; }

		public IList<string> laborDesc { get; set; }

		public string invoiceLink { get; set; }
	}
}
