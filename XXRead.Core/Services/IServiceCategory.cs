using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XXRead.Core.Models;

namespace XXRead.Core.Services
{
	public interface IServiceCategory
	{
		Task InitCategories();
		Task<List<Category>> GetCategoriesDB();
		bool HasCategorySelectionChanged(Category currentCategory);
		void SetCurrentCategory(Category currentCategory);
		Category GetCurrentCategory();
	}
}
