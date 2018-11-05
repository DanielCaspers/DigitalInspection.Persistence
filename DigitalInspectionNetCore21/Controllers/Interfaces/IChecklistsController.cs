using DigitalInspectionNetCore21.Controllers.Interfaces.RepositoryActions;
using DigitalInspectionNetCore21.Models.Web;
using DigitalInspectionNetCore21.Models.Web.Checklists;
using DigitalInspectionNetCore21.Models.Web.Checklists.Requests;

namespace DigitalInspectionNetCore21.Controllers.Interfaces
{
	internal interface IChecklistItemsController: 
		IGetAll<ChecklistItemSummary>,
		IGetById<ChecklistItem>,
		ICreate<AddChecklistItemRequest, ChecklistItem>,
		IUpdate<EditChecklistItemRequest>,
		IDelete
	{
	}
}
