using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XStory.DTO;
using XStory.Logger;
using XXRead.Core.Session;
using XXRead.Core.Sources;
using XXRead.Core.Storage;

namespace XXRead.Core.Services
{
	public class ServiceCategory : IServiceCategory
	{
		private IServiceSource _serviceSource;
		private IServiceSession _serviceSession;
		private IRepositoryCategory _repositoryCategory;

		public ServiceCategory(IServiceSource serviceSource,
			IServiceSession serviceSession,
			IRepositoryCategory repositoryCategory)
		{
			_serviceSource = serviceSource;
			_serviceSession = serviceSession;
			_repositoryCategory = repositoryCategory;
		}

		/// <summary>
		/// Checks if has categories in local db.
		/// If not, get Categories from web and insert it in the database.
		/// </summary>
		public async Task InitCategories()
		{
			try
			{
				bool hasDbCategories = await _repositoryCategory.HasCategories(_serviceSession.DataSource);

				if (!hasDbCategories)
				{
					List<Category> categories = await _serviceSource.Current.GetCategories();
					await _repositoryCategory.InsertCategories(categories);
				}
			}
			catch (Exception ex)
			{
				ServiceLog.Error(ex);
			}
		}

		/// <summary>
		/// Get categories from database
		/// </summary>
		/// <returns></returns>
		public async Task<List<Category>> GetCategoriesDB()
		{
			List<Category> categories;
			try
			{
				// Categories from SQLite
				categories = await _repositoryCategory.GetCategories(_serviceSession.DataSource);
				if (categories == null || categories.Count == 0)
				{
					throw new Exception("No categories in database");
				}

				return categories.OrderBy(c => c.Title).ToList();

			}
			catch (Exception ex)
			{
				ServiceLog.Error(ex);
				categories = null;
			}
			return categories;
		}

		/// <summary>
		/// Compares if the session's and parameter's category are same.
		/// </summary>
		/// <param name="currentCategory"></param>
		/// <returns></returns>
		public bool HasCategorySelectionChanged(Category currentCategory)
		{
			return object.Equals(_serviceSession.CurrentCategory, currentCategory) ? false : true;
		}

		public void SetCurrentCategory(Category currentCategory)
		{
			_serviceSession.CurrentCategory = currentCategory;
		}

		public Category GetCurrentCategory()
		{
			return _serviceSession.CurrentCategory;
		}
	}
}
