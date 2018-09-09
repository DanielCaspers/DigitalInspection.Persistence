using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace DigitalInspectionNetCore21.Controllers
{
	public interface IRepositoryController<TEntity, TCreateModel, TUpdateModel>
	{
		ActionResult<IEnumerable<TEntity>> GetAll();

		ActionResult<TEntity> GetById(Guid id);

		ActionResult<TEntity> Create(TCreateModel request);

		ActionResult<TEntity> Update(Guid id, TUpdateModel request);

		NoContentResult Delete(Guid id);
	}
}