using DigitalInspectionNetCore21.Controllers.Interfaces.RepositoryActions;
using DigitalInspectionNetCore21.Models.Web;
using DigitalInspectionNetCore21.Models.Web.Checklists;
using DigitalInspectionNetCore21.Models.Web.Checklists.Requests;

namespace DigitalInspectionNetCore21.Controllers.Interfaces
{
	internal interface ITagsController: 
		IGetAll<Tag>,
		IGetById<Tag>,
		ICreate<AddTagRequest, Tag>,
		IUpdate<AddTagRequest>,
		IDelete
	{
	}
}
