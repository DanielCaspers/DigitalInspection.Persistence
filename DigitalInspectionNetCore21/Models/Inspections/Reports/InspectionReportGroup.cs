using System.Collections.Generic;
using System.Linq;

namespace DigitalInspectionNetCore21.Models.Inspections.Reports
{
	public class InspectionReportGroup
	{
		public string Name { get; set; }

		public InspectionItemCondition Condition { get; set; }

		public IEnumerable<InspectionReportItem> Items { get; set; }

		public InspectionReportGroup(IGrouping<string, InspectionItem> ig, string baseUrl)
		{
			var items = ig.OrderBy(ii => ii.Condition).ToList();

			Name = ig.Key;
			Condition = items.First().Condition;
			Items = items.Select(ii => new InspectionReportItem(ii, baseUrl));
		}
	}
}
