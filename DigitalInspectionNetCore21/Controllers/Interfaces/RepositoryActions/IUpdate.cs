using System;
using Microsoft.AspNetCore.Mvc;

namespace DigitalInspectionNetCore21.Controllers.Interfaces.RepositoryActions
{
	internal interface IUpdate<in TRequest>
	{
		ActionResult Update(Guid id, TRequest request);
	}
}
