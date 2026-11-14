using XXRead.Core.Models;

namespace XXRead.Core.Storage
{
	/// <summary>
	/// Local cache of each source's categories.
	/// </summary>
	public interface IRepositoryCategory
	{
		Task<bool> HasCategories(DataSources source);
		Task<List<Category>> GetCategories(DataSources source);
		Task<int> InsertCategories(List<Category> categories);
	}
}
