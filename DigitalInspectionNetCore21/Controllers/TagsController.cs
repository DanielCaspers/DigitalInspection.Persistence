using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using DigitalInspectionNetCore21.Controllers.Interfaces;
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
		public ActionResult<IEnumerable<TagResponse>> GetAll()
		{
			var tags = _tagRepository.GetAll().ToList();
			return GetTagsResponse(tags);
		}

		/// <summary>
		/// Get all tags which are visible to customers
		/// </summary>
		/// <response code="200">Ok</response>
		[HttpGet("VisibleToCustomer")]
		public ActionResult<IEnumerable<TagResponse>> GetAllCustomerVisible()
		{
			var tags = _tagRepository.GetAllCustomerVisible().ToList();
			return GetTagsResponse(tags);
		}

		/// <summary>
		/// Get all tags which are visible to employees
		/// </summary>
		/// <response code="200">Ok</response>
		[HttpGet("VisibleToEmployee")]
		public ActionResult<IEnumerable<TagResponse>> GetAllEmployeeVisible()
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

		/// <summary>
		///	Update a tag
		/// </summary>
		/// <param name="id"></param>
		/// <param name="tag">Tag description</param>
		/// <response code="204">No content</response>
		/// <response code="404">Tag not found</response>
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
