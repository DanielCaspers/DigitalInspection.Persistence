using DigitalInspectionNetCore21.Controllers.Interfaces.RepositoryActions;
using DigitalInspectionNetCore21.Models.Web;
using DigitalInspectionNetCore21.Models.Web.Inspections;

namespace DigitalInspectionNetCore21.Controllers.Interfaces
{
	internal interface IInspectionItemsController: 
		IGetById<InspectionItem>
	{
	}
}
