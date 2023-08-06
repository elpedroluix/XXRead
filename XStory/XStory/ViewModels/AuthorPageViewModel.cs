using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using XStory.DTO;
using XStory.Logger;

namespace XStory.ViewModels
{
	public class AuthorPageViewModel : BaseViewModel
	{
		#region --- Fields ---
		private Author _author;
		public Author Author
		{
			get { return _author; }
			set { SetProperty(ref _author, value); }
		}
		#endregion

		#region --- Ctor ---
		public AuthorPageViewModel(INavigationService navigationService) : base(navigationService)
		{
			ViewState = Helpers.ViewStateEnum.Loading;
		}
		#endregion

		private async void InitAuthor(Author author)
		{
			try
			{
				if (author == null)
				{
					throw new Exception("Author cannot be null");
				}

				Author = author;
				Title = Author.Name;

				// _serviceAuthor.GetAuthorInfos
				// _serviceAuthor.GetAuthorStories
				// ou réunies en 1 seule methode...
				// To be continued...
			}
			catch (Exception ex)
			{
				ServiceLog.Error(ex);
			}

			ViewState = Helpers.ViewStateEnum.Display;
		}

		public override void OnNavigatedTo(INavigationParameters parameters)
		{
			var author = parameters.GetValue<DTO.Author>("author");

			InitAuthor(author);
		}
	}
}
