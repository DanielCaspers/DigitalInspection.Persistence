namespace DigitalInspectionNetCore21.Models.Inspections
{
	public enum InspectionItemCondition
	{
		// The unset value of the enum should be 0, so that this is also the default constructed value
		// We need this to remain this way so that we can filter these out from the customer's view
		UNKNOWN = 0,

		IMMEDIATE = 1,

		MODERATE = 2,

		SHOULD_WATCH = 3,

		MAINTENANCE = 4,

		NOTES = 5,

		OK = 10,

		NOT_APPLICABLE = 11
	}
}