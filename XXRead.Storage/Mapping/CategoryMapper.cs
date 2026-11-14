using XXRead.Core.Models;

namespace XXRead.Storage.Mapping
{
	internal class CategoryMapper
	{
		public static Category ToCategoryDTO(Entities.Category category)
		{
			return new Category()
			{
				Title = category.Title,
				Source = category.Source,
				Url = category.Url
			};
		}

		public static Entities.Category ToCategorySQLite(Category category)
		{
			return new Entities.Category()
			{
				Title = category.Title,
				Source = category.Source,
				Url = category.Url
			};
		}
	}
}
