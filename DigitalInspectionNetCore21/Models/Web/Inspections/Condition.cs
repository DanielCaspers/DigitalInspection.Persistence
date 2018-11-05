namespace DigitalInspectionNetCore21.Models.Web.Inspections
{
	public class Condition: TypeSafeEnum
	{
		// The unset value of the enum should be 0, so that this is also the default constructed value
		// We need this to remain this way so that we can filter these out from the customer's view
		public static readonly Condition Unknown = new Condition(0, "Unknown");
		public static readonly Condition Immediate = new Condition(1, "Immediate");
		public static readonly Condition Moderate = new Condition(2, "Moderate");
		public static readonly Condition ShouldWatch = new Condition(3, "Should watch");
		public static readonly Condition Maintenance = new Condition(4, "Maintenance");
		public static readonly Condition Notes = new Condition(5, "Notes");
		public static readonly Condition Ok = new Condition(10, "Ok");
		public static readonly Condition NotApplicable = new Condition(11, "Not applicable");

		public Condition(){}

		private Condition(int value, string displayName) : base(value, displayName) { }
	}
}
