using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace DigitalInspectionNetCore21.Controllers.Interfaces.RepositoryActions
{
	internal interface IGetAll<T>
	{
		ActionResult<IEnumerable<T>> GetAll();
	}
}
