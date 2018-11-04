using DigitalInspectionNetCore21.Models.Web.Inspections;
using DigitalInspectionNetCore21.ViewModels.Checklists;

namespace DigitalInspectionNetCore21.Controllers.Interfaces
{
	internal interface IChecklistsController: 
		IGetAll<ChecklistSummaryResponse>,
		IGetById<ChecklistResponse>,
		ICreate<AddChecklistViewModel, ChecklistResponse>,
		IUpdate<EditChecklistViewModel>,
		IDelete
	{
	}
}
