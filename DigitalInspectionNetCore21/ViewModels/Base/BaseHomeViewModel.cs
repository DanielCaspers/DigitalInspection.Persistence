namespace DigitalInspectionNetCore21.ViewModels.Base
{
	public class BaseHomeViewModel : BaseViewModel
	{
		public BaseHomeViewModel()
		{
			ResourceName = "Home";
			ResourceControllerName = "Home";
			ResourceMethodName = "Index";
		}
	}
}