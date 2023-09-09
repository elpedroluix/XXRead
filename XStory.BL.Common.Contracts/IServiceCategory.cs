using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XStory.BL.Common.Contracts
{
	public interface IServiceCategory
	{
		Task InitCategories();
		Task<List<DTO.Category>> GetCategoriesDB(bool includeHidden = false);
		Task<int> Save(DTO.Category category);
		Task InitHiddenCategories();
		bool HasCategorySelectionChanged(DTO.Category currentCategory);
		void SetCurrentCategory(DTO.Category currentCategory);
		DTO.Category GetCurrentCategory();
	}
}
