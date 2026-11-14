using XStory.DTO;
using XStory.DTO.Config;
using XXRead.Core.Storage;
using XXRead.Storage.Mapping;

namespace XXRead.Storage
{
	internal class RepositoryCategory : Repository, IRepositoryCategory
	{
		public RepositoryCategory(XXReadDatabase database) : base(database)
		{
		}

		public async Task<bool> HasCategories(DataSources source)
		{
			List<Entities.Category> categories = await this.GetCategoriesFromSource(source);
			return categories.Count > 0;
		}

		public async Task<List<Category>> GetCategories(DataSources source)
		{
			List<Entities.Category> sqliteCategories = await this.GetCategoriesFromSource(source);

			return sqliteCategories.Select(CategoryMapper.ToCategoryDTO).ToList();
		}

		public async Task<int> InsertCategories(List<Category> categories)
		{
			List<Entities.Category> sqliteCategories = categories.Select(CategoryMapper.ToCategorySQLite).ToList();

			return await _database.GetInstance().InsertAllAsync(sqliteCategories, true);
		}

		private async Task<List<Entities.Category>> GetCategoriesFromSource(DataSources source)
		{
			// Same values as Category.Source, set by the sources ("HDS", "XStory")
			string sourceName = source.ToString();

			return await _database.GetInstance().Table<Entities.Category>()
				.Where(c => c.Source == sourceName)
				.ToListAsync();
		}
	}
}
