using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DigitalInspectionNetCore21.Models.Web.Checklists.Requests
{
	public class AddChecklistRequest
	{
		[Required(ErrorMessage = "Checklist name is required")]
		[DisplayName("Checklist name *")]
		public string Name { get; set; }
	}
}