using XXRead.Core.Models;

namespace XXRead.UI.Dialogs
{
	/// <summary>
	/// Result of <see cref="DialogSelectCategory"/>. A null result means the dialog was canceled,
	/// a null <see cref="Category"/> means "all categories".
	/// </summary>
	public class CategorySelection
	{
		public Category Category { get; private set; }

		public CategorySelection(Category category)
		{
			Category = category;
		}
	}
}
