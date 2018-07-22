// FIXME DJC This is probably done differently here, revisit

using System;
using System.Diagnostics;

namespace DigitalInspectionNetCore21.Services
{
	// TODO Review with C# Devs at work to find a better solution. Perhaps a global event observer?
	public static class ExceptionHandlerService
	{
		public static void HandleException(Exception dbEx)
		{
			Trace.TraceError("Property: {0} Error: {1}",
				dbEx.Data,
				dbEx.Message);
			//foreach (var validationErrors in dbEx.EntityValidationErrors)
			//{
			//	foreach (var validationError in validationErrors.ValidationErrors)
			//	{
			//		Trace.TraceError("Property: {0} Error: {1}",
			//								validationError.PropertyName,
			//								validationError.ErrorMessage);
			//	}
			//}
		}
	}
}
