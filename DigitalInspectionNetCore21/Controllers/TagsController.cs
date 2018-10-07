using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using DigitalInspectionNetCore21.Models.DbContexts;
using DigitalInspectionNetCore21.Models.Inspections;
using DigitalInspectionNetCore21.Models.Web.Inspections;
using DigitalInspectionNetCore21.Services.Core.Interfaces;
using DigitalInspectionNetCore21.ViewModels.Tags;
using Microsoft.AspNetCore.Mvc;

namespace DigitalInspectionNetCore21.Controllers
{
	//[AuthorizeRoles(Roles.Admin)]
	[Route("[controller]")]
	public class TagsController : BaseController
	{
		private readonly ITagRepository _tagRepository;

		public TagsController(ApplicationDbContext db, ITagRepository tagRepository) : base(db)
		{
			_tagRepository = tagRepository;
		}

		[HttpGet("")]
		public ActionResult<IEnumerable<TagResponse>> GetAll()
		{
			var tags = _tagRepository.GetAll().ToList();
			return GetTagsResponse(tags);
		}

		[HttpGet("VisibleToCustomer")]
		public ActionResult<IEnumerable<TagResponse>> GetAllCustomerVisible()
		{
			var tags = _tagRepository.GetAllCustomerVisible().ToList();
			return GetTagsResponse(tags);
		}

		[HttpGet("VisibleToEmployee")]
		public ActionResult<IEnumerable<TagResponse>> GetAllEmployeeVisible()
		{
			var tags = _tagRepository.GetAllEmployeeVisible().ToList();
			return GetTagsResponse(tags);
		}

		[HttpGet("{id}")]
		public ActionResult<TagResponse> GetById(Guid id)
		{
			var tag = _context.Tags.Find(id);

			if (tag == null)
			{
				return NotFound();
			}

			var tagResponse = Mapper.Map<TagResponse>(tag);

			return Json(tagResponse);
		}

		[HttpPost("")]
		public ActionResult<TagResponse> Create([FromBody]AddTagViewModel request)
		{
			var tag = new Models.Inspections.Tag
			{
				Name = request.Name,
				IsVisibleToCustomer = request.IsVisibleToCustomer,
				IsVisibleToEmployee = request.IsVisibleToEmployee
			};

			_context.Tags.Add(tag);
			_context.SaveChanges();

			var tagResponse = Mapper.Map<TagResponse>(tag);

			var createdUri = new Uri(HttpContext.Request.Path, UriKind.Relative);
			return Created(createdUri, tagResponse);
		}

		[HttpPut("{id}")]
		public ActionResult Update(Guid id, [FromBody]AddTagViewModel tag)
		{
			var tagInDb = _context.Tags.SingleOrDefault(t => t.Id == id);
			if(tagInDb == null)
			{
				return NotFound();
			}

			tagInDb.Name = tag.Name;
			tagInDb.IsVisibleToCustomer = tag.IsVisibleToCustomer;
			tagInDb.IsVisibleToEmployee = tag.IsVisibleToEmployee;

			_context.SaveChanges();

			return NoContent();
		}

		[HttpDelete("{id}")]
		public NoContentResult Delete(Guid id)
		{
			var tagInDb = _context.Tags.Find(id);

			if (tagInDb != null)
			{
				_context.Tags.Remove(tagInDb);
				_context.SaveChanges();
			}

			return NoContent();
		}

		private JsonResult GetTagsResponse(IEnumerable<Tag> tags)
		{
			var tagResponse = Mapper.Map<IEnumerable<TagResponse>>(tags);

			return Json(tagResponse);
		}
	}
}
