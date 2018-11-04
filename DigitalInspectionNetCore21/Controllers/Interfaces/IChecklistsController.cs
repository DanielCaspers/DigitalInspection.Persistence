using DigitalInspectionNetCore21.Models.Web.Inspections;
using DigitalInspectionNetCore21.ViewModels.ChecklistItems;

namespace DigitalInspectionNetCore21.Controllers.Interfaces
{
	internal interface IChecklistItemsController: 
		IGetAll<ChecklistItemSummaryResponse>,
		IGetById<ChecklistItemResponse>,
		ICreate<AddChecklistItemViewModel, ChecklistItemResponse>,
		IUpdate<EditChecklistItemViewModel>,
		IDelete
	{
	}
}
