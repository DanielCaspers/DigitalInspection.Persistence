using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DigitalInspectionNetCore21.Models.Validators;
using Microsoft.AspNetCore.Http;

namespace DigitalInspectionNetCore21.ViewModels.Checklists
{
	public class AddChecklistViewModel
	{
		[Required(ErrorMessage = "Checklist name is required")]
		[DisplayName("Checklist name *")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Picture is required")]
		[DisplayName("Attach a picture *")]
		[MaxFileSize(8 * 1024 * 1024, ErrorMessage = "Max image size is 8 MB")]
		[DataType(DataType.Upload)]
		public IFormFile Picture { get; set; }
	}
}