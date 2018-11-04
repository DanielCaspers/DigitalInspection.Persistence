using System;
using Microsoft.AspNetCore.Mvc;

namespace DigitalInspectionNetCore21.Controllers.Interfaces
{
	internal interface IGetById<T>
	{
		ActionResult<T> GetById(Guid id);
	}
}
