using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using DigitalInspectionNetCore21.Controllers.Interfaces;
using DigitalInspectionNetCore21.Models.DbContexts;
using DigitalInspectionNetCore21.Models.Inspections;
using DigitalInspectionNetCore21.Models.Web;
using DigitalInspectionNetCore21.Models.Web.Checklists;
using DigitalInspectionNetCore21.Models.Web.Checklists.Requests;
using DigitalInspectionNetCore21.Services.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Tag = DigitalInspectionNetCore21.Models.Web.Checklists.Tag;

namespace DigitalInspectionNetCore21.Controllers
{
	//[AuthorizeRoles(Roles.Admin)]
	[Route("[controller]")]
	public class TagsController : BaseController, ITagsController
	{
		private readonly ITagRepository _tagRepository;

		public TagsController(ApplicationDbContext db, ITagRepository tagRepository) : base(db)
		{
			_tagRepository = tagRepository;
		}

		/// <summary>
		/// Get all tags
		/// </summary>
		/// <response code="200">Ok</response>
		[HttpGet("")]
		[ProducesResponseType(200)]
		public ActionResult<IEnumerable<Tag>> GetAll()
		{
			var tags = _tagRepository.GetAll().ToList();
			return GetTagsResponse(tags);
		}

		/// <summary>
		/// Get all tags which are visible to customers
		/// </summary>
		/// <response code="200">Ok</response>
		[HttpGet("VisibleToCustomer")]
		[ProducesResponseType(200)]
		public ActionResult<IEnumerable<Tag>> GetAllCustomerVisible()
		{
			var tags = _tagRepository.GetAllCustomerVisible().ToList();
			return GetTagsResponse(tags);
		}

		/// <summary>
		/// Get all tags which are visible to employees
		/// </summary>
		/// <response code="200">Ok</response>
		[HttpGet("VisibleToEmployee")]
		[ProducesResponseType(200)]
		public ActionResult<IEnumerable<Tag>> GetAllEmployeeVisible()
		{
			var tags = _tagRepository.GetAllEmployeeVisible().ToList();
			return GetTagsResponse(tags);
		}

		/// <summary>
		/// Get tag
		/// </summary>
		/// <param name="id"></param>
		/// <response code="200">Ok</response>
		/// <response code="404">Not found</response>
		[HttpGet("{id}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		public ActionResult<Tag> GetById(Guid id)
		{
			var tag = _context.Tags.Find(id);

			if (tag == null)
			{
				return NotFound();
			}

			var tagResponse = Mapper.Map<Tag>(tag);

			return Json(tagResponse);
		}

		/// <summary>
		/// Create a tag for grouping checklist items under
		/// </summary>
		/// <remarks>
		/// The tags feature has multiple uses
		/// - Allow employees to complete inspections by looking through the entire list, 
		/// or by filtering down to relevant tags
		/// - Use tag visibility to remove certain items from being sent out in an inspection
		/// </remarks>
		/// <param name="request">Tag description</param>
		/// <response code="201">Created</response>
		[HttpPost("")]
		[ProducesResponseType(201)]
		public ActionResult<Tag> Create([FromBody]AddTagRequest request)
		{
			var tag = new Models.Inspections.Tag
			{
				Name = request.Name,
				IsVisibleToCustomer = request.IsVisibleToCustomer,
				IsVisibleToEmployee = request.IsVisibleToEmployee
			};

			_context.Tags.Add(tag);
			_context.SaveChanges();

			var tagResponse = Mapper.Map<Tag>(tag);

			var createdUri = new Uri(HttpContext.Request.Path, UriKind.Relative);
			return Created(createdUri, tagResponse);
		}

		/// <summary>
		///	Update a tag
		/// </summary>
		/// <param name="id"></param>
		/// <param name="tag">Tag description</param>
		/// <response code="204">No content</response>
		/// <response code="404">Tag not found</response>
		[HttpPut("{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		public ActionResult Update(Guid id, [FromBody]AddTagRequest tag)
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

		/// <summary>
		/// Delete a tag
		/// </summary>
		/// <remarks>
		/// Tags only serve as metadata for checklist items. 
		/// Thus, deleting them has no impact on inspections reports.
		/// </remarks>
		/// <param name="id"></param>
		/// <response code="204">No content</response>
		[HttpDelete("{id}")]
		[ProducesResponseType(204)]
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

		private JsonResult GetTagsResponse(IEnumerable<Models.Inspections.Tag> tags)
		{
			var tagResponse = Mapper.Map<IEnumerable<Tag>>(tags);

			return Json(tagResponse);
		}
	}
}
