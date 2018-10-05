using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace DigitalInspectionNetCore21.Controllers
{
	// TODO: Consider scrapping this whole thing, since summary types really screw with genericism
	public interface IRepositoryController<TEntity, TCreateModel, TUpdateModel>
	{
		//ActionResult<IEnumerable<TEntity>> GetAll();

		ActionResult<TEntity> GetById(Guid id);

		ActionResult<TEntity> Create(TCreateModel request);

		ActionResult Update(Guid id, TUpdateModel request);

		NoContentResult Delete(Guid id);
	}
}