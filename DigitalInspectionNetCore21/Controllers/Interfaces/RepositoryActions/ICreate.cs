using Microsoft.AspNetCore.Mvc;

namespace DigitalInspectionNetCore21.Controllers.Interfaces.RepositoryActions
{
	internal interface ICreate<TRequest, TResponse>
	{
		ActionResult<TResponse> Create(TRequest request);
	}
}
