using DigitalInspectionNetCore21.Models.Web;

namespace DigitalInspectionNetCore21.Models.Inspections
{
	public class InspectionItemCondition : TypeSafeEnum
	{
		// The unset value of the enum should be 0, so that this is also the default constructed value
		// We need this to remain this way so that we can filter these out from the customer's view
		public static readonly InspectionItemCondition Unknown = new InspectionItemCondition(0, "Unknown");
		public static readonly InspectionItemCondition Immediate = new InspectionItemCondition(1, "Immediate");
		public static readonly InspectionItemCondition Moderate = new InspectionItemCondition(2, "Moderate");
		public static readonly InspectionItemCondition ShouldWatch = new InspectionItemCondition(3, "Should watch");
		public static readonly InspectionItemCondition Maintenance = new InspectionItemCondition(4, "Maintenance");
		public static readonly InspectionItemCondition Notes = new InspectionItemCondition(5, "Notes");
		public static readonly InspectionItemCondition Ok = new InspectionItemCondition(10, "Ok");
		public static readonly InspectionItemCondition NotApplicable = new InspectionItemCondition(11, "Not applicable");

		public InspectionItemCondition() { }

		private InspectionItemCondition(int value, string displayName) : base(value, displayName) { }
	}
}
