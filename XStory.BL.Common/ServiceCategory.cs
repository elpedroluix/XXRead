using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XStory.BL.Common.Contracts;
using XStory.DTO;

namespace XStory.BL.Common
{
	public class ServiceCategory : IServiceCategory
	{
		private BL.Web.DSLocator.Contracts.IServiceCategory _dsServiceCategoryWeb;
		private BL.SQLite.Contracts.IServiceCategory _serviceCategorySQLite;

		public ServiceCategory(BL.Web.DSLocator.Contracts.IServiceCategory dsServiceCategoryWeb,
			BL.SQLite.Contracts.IServiceCategory serviceCategorySQLite)
		{
			_dsServiceCategoryWeb = dsServiceCategoryWeb;
			_serviceCategorySQLite = serviceCategorySQLite;
		}

		/// <summary>
		/// Checks if has categories in local db.</br>
		/// If not, get Categories from web and insert it in the database.
		/// </summary>
		public async Task InitCategories()
		{
			try
			{
				bool hasDbCategories = await _serviceCategorySQLite.HasDBCategories(StaticContext.DataSource.ToString());

				if (!hasDbCategories)
				{
					List<DTO.Category> categories = await _dsServiceCategoryWeb.GetCategories(StaticContext.DataSource.ToString());
					await _serviceCategorySQLite.InsertCategories(categories);
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
		public async Task<List<Category>> GetCategoriesDB(bool includeHidden = false)
		{
			List<DTO.Category> categories;
			try
			{
				// Categories from SQLite
				categories = await _serviceCategorySQLite.GetCategories(StaticContext.DataSource.ToString(), includeHidden);
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

		public async Task InitHiddenCategories()
		{
			StaticContext.HiddenCategories = await _serviceCategorySQLite.GetHiddenCategories(StaticContext.DataSource.ToString());
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

		public async Task<int> Save(Category category)
		{
			return await _serviceCategorySQLite.Save(category);
		}
	}
}
