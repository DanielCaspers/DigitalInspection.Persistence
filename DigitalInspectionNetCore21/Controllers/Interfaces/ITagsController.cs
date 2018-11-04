using DigitalInspectionNetCore21.Models.Web.Inspections;
using DigitalInspectionNetCore21.ViewModels.ChecklistItems;
using DigitalInspectionNetCore21.ViewModels.Tags;

namespace DigitalInspectionNetCore21.Controllers.Interfaces
{
	internal interface ITagsController: 
		IGetAll<TagResponse>,
		IGetById<TagResponse>,
		ICreate<AddTagViewModel, TagResponse>,
		IUpdate<AddTagViewModel>,
		IDelete
	{
	}
}
