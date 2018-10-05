
namespace DigitalInspectionNetCore21.Models.Web.Orders
{
	public enum RecommendedServiceSeverity
	{
		// The unset value of the enum should be 0, so that this is also the default constructed value
		// We need this to remain this way so that we can filter these out from the customer's view
		Unknown = 0,

		Immediate = 1,

		Moderate = 2,

		ShouldWatch = 3,

		Maintenance = 4,

		Notes = 5,

		Ok = 10,

		NotApplicable = 11
	}
}
