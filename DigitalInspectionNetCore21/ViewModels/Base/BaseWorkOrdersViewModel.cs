namespace DigitalInspectionNetCore21.ViewModels.Base
{
	public class BaseWorkOrdersViewModel : BaseViewModel
	{
		public BaseWorkOrdersViewModel()
		{
			ResourceName = "Work Orders";
			ResourceControllerName = "WorkOrders";
			ResourceMethodName = "Index";
		}
	}
}