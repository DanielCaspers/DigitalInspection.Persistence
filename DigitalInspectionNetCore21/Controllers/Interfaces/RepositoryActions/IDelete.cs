using System;
using Microsoft.AspNetCore.Mvc;

namespace DigitalInspectionNetCore21.Controllers.Interfaces
{
	internal interface IDelete
	{
		NoContentResult Delete(Guid id);
	}
}
