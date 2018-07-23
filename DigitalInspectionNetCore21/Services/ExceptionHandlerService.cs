// TODO DJC Make a global exception pipeline

using System;
using System.Diagnostics;

namespace DigitalInspectionNetCore21.Services
{
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
