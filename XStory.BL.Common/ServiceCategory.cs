using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XStory.BL.Common.Contracts;
using XStory.DTO;
using XXRead.Core.Sources;
using XXRead.Core.Storage;

namespace XStory.BL.Common
{
	public class ServiceCategory : IServiceCategory
	{
		private IServiceSource _serviceSource;
		private IRepositoryCategory _repositoryCategory;

		public ServiceCategory(IServiceSource serviceSource,
			IRepositoryCategory repositoryCategory)
		{
			_serviceSource = serviceSource;
			_repositoryCategory = repositoryCategory;
		}

		/// <summary>
		/// Checks if has categories in local db.</br>
		/// If not, get Categories from web and insert it in the database.
		/// </summary>
		public async Task InitCategories()
		{
			try
			{
				bool hasDbCategories = await _repositoryCategory.HasCategories(StaticContext.DataSource);

				if (!hasDbCategories)
				{
					List<DTO.Category> categories = await _serviceSource.Get(StaticContext.DataSource).GetCategories();
					await _repositoryCategory.InsertCategories(categories);
				}
			}
			catch (Exception ex)
			{
				Logger.ServiceLog.Error(ex);
			}
		}

		/// <summary>
		/// Get categories from database </br>
		/// </summary>
		/// <returns></returns>
		public async Task<List<Category>> GetCategoriesDB()
		{
			List<DTO.Category> categories;
			try
			{
				// Categories from SQLite
				categories = await _repositoryCategory.GetCategories(StaticContext.DataSource);
				if (categories == null || categories.Count == 0)
				{
					throw new Exception("No categories in database");
				}

				return categories.OrderBy(c => c.Title).ToList();

			}
			catch (Exception ex)
			{
				Logger.ServiceLog.Error(ex);
				categories = null;
			}
			return categories;
		}

		/// <summary>
		/// Compares if StaticContext and parameter's category are same.</br>
		/// </summary>
		/// <param name="currentCategory"></param>
		/// <returns></returns>
		public bool HasCategorySelectionChanged(Category currentCategory)
		{
			return object.Equals(StaticContext.CurrentCategory, currentCategory) ? false : true;
		}

		public void SetCurrentCategory(Category currentCategory)
		{
			StaticContext.CurrentCategory = currentCategory;
		}

		public Category GetCurrentCategory()
		{
			return StaticContext.CurrentCategory;
		}
	}
}
