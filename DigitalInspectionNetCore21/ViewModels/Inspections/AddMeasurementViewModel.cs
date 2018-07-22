using System.Collections.Generic;
using DigitalInspectionNetCore21.Models.Inspections;

namespace DigitalInspectionNetCore21.ViewModels.Inspections
{
	public class AddMeasurementViewModel
	{
		public ChecklistItem ChecklistItem { get; set; }
		public InspectionItem InspectionItem { get; set; }
		public IList<Measurement> Measurements { get; set; }
	}
}