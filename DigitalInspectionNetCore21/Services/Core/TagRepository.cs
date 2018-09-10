using System.Linq;
using DigitalInspectionNetCore21.Models.DbContexts;
using DigitalInspectionNetCore21.Models.Inspections;
using DigitalInspectionNetCore21.Services.Core.Interfaces;

namespace DigitalInspectionNetCore21.Services.Core
{
	/// <inheritdoc cref="ITagRepository"/>
	public class TagRepository : ITagRepository
	{
		private readonly ApplicationDbContext _context;

		public TagRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public IQueryable<Tag> GetAll()
		{
			return _context.Tags.OrderBy(t => t.Name);
		}

		public IQueryable<Tag> GetAllEmployeeVisible()
		{
			return GetAll().Where(t => t.IsVisibleToEmployee);
		}

		public IQueryable<Tag> GetAllCustomerVisible()
		{
			return GetAll().Where(t => t.IsVisibleToCustomer);
		}
	}
}
