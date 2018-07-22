using System.Collections.Generic;
using DigitalInspectionNetCore21.Models.Inspections;
using DigitalInspectionNetCore21.ViewModels.Base;

namespace DigitalInspectionNetCore21.ViewModels.Tags
{
	public class ManageTagsViewModel: BaseChecklistsViewModel
	{
		public List<Tag> Tags { get; set; }
		public AddTagViewModel AddTagVM { get; set; }
	}
}