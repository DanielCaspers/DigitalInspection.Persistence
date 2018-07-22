using System.Collections.Generic;
using DigitalInspectionNetCore21.Models.DTOs;
using DigitalInspectionNetCore21.Models.Orders;
using DigitalInspectionNetCore21.Utils;

namespace DigitalInspectionNetCore21.Models.Mappers
{
	public static class VehicleHistoryItemMapper
	{
		public static IList<VehicleHistoryItem> mapToVehicleHistoryItems(VehicleHistoryItemDTO[] dto)
		{
			var historyItems = new List<VehicleHistoryItem>();

			foreach (var itemDto in dto)
			{
				historyItems.Add(new VehicleHistoryItem
				{
					OrderId = itemDto.orderID,
					CompletionDate = DateTimeUtils.FromUnixTime(itemDto.completionDate),
					VehicleOdometer = itemDto.vehicleOdometer,
					LaborDescription = itemDto.laborDesc,
					InvoiceLink = itemDto.invoiceLink
				});
			}

			return historyItems;
		}
	}
}
