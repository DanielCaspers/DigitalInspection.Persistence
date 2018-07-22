using System.Diagnostics;
//using System.Web.Mvc;

namespace DigitalInspectionNetCore21.ViewModels.Base
{
	public class BaseErrorModel : BaseViewModel
	{
		// FIXME DJC This is different now
		// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/error-handling?view=aspnetcore-2.1
		//public HandleErrorInfo Error { get; set; }

		public StackTrace StackTrace { get; set; }
	}
}