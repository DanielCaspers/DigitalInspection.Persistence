using System.Linq;
using DigitalInspectionNetCore21.Models.Inspections;

namespace DigitalInspectionNetCore21.Services.Core.Interfaces
{
	/// <summary>
	/// Used for facading query logic for tags
	/// </summary>
	/// <inheritdoc cref="ITagRepository"/>
	public interface ITagRepository
	{
		/// <summary>
		/// Gets all tags, sorted by name
		/// </summary>
		IQueryable<Tag> GetAll();

		/// <summary>
		/// Gets all customer visible tags, sorted by name
		/// </summary>
		IQueryable<Tag> GetAllCustomerVisible();

		/// <summary>
		/// Gets all employee visible tags, sorted by name
		/// </summary>
		IQueryable<Tag> GetAllEmployeeVisible();
	}
}