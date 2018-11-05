using System;
using Microsoft.AspNetCore.Mvc;

namespace DigitalInspectionNetCore21.Controllers.Interfaces.RepositoryActions
{
	internal interface IGetById<T>
	{
		ActionResult<T> GetById(Guid id);
	}
}
