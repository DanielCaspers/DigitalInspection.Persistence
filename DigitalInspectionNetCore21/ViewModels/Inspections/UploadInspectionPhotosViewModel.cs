using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DigitalInspectionNetCore21.Models.Inspections;
using DigitalInspectionNetCore21.Models.Validators;
using Microsoft.AspNetCore.Http;

namespace DigitalInspectionNetCore21.ViewModels.Inspections
{
	public class UploadInspectionPhotosViewModel
	{
		public ChecklistItem ChecklistItem { get; set; }

		public InspectionItem InspectionItem { get; set; }

		// Used for Post-Return-Get
		public Guid ChecklistId { get; set; }
		public Guid? TagId { get; set; }

		// Used for the naming scheme of the saved image for easier recognition outside of DI
		// for the use case of pruning old inspection images which no longer need to be kept on file.
		public string WorkOrderId { get; set; }

		[Required(ErrorMessage = "Picture is required")]
		[DisplayName("Attach a picture *")]
		[MaxFileSize(8 * 1024 * 1024, ErrorMessage = "Max image size is 8 MB")]
		[DataType(DataType.Upload)]
		public IFormFile Picture { get; set; }
	}
}