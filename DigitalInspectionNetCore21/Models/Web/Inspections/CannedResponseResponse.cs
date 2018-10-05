using System;
using System.Collections.Generic;
using DigitalInspectionNetCore21.Models.Orders;

namespace DigitalInspectionNetCore21.Models.Web.Inspections
{
	public class CannedResponseResponse
	{
		public Guid Id { get; set; }

		public string Response { get; set; }

		public string Url { get; set; }

		public string Description { get; set; }

		public IList<RecommendedServiceSeverity> LevelsOfConcern { get; set; }
	}
}