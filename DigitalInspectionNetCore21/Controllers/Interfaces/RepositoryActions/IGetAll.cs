using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace DigitalInspectionNetCore21.Controllers.Interfaces
{
	internal interface IGetAll<T>
	{
		ActionResult<IEnumerable<T>> GetAll();
	}
}
