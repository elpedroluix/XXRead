using XXRead.Core.Models;
using XXRead.Core.Services;

namespace XXRead.Tests.UI.Fakes
{
	public class FakeServiceCategory : IServiceCategory
	{
		public List<Category> Categories { get; set; } = new List<Category>();

		public Category CurrentCategory { get; set; }

		public Task InitCategories() => Task.CompletedTask;

		public Task<List<Category>> GetCategoriesDB() => Task.FromResult(Categories);

		public bool HasCategorySelectionChanged(Category currentCategory) => currentCategory != CurrentCategory;

		public void SetCurrentCategory(Category currentCategory) => CurrentCategory = currentCategory;

		public Category GetCurrentCategory() => CurrentCategory;
	}
}
