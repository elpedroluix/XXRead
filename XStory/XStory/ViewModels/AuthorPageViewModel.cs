using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using XStory.DTO;
using XStory.Helpers;
using XStory.Logger;

namespace XStory.ViewModels
{
	public class AuthorPageViewModel : BaseViewModel
	{
		#region --- Fields ---

		BL.Web.DSLocator.Contracts.IServiceAuthor _serviceAuthor;

		private Author _author;
		public Author Author
		{
			get { return _author; }
			set { SetProperty(ref _author, value); }
		}
		#endregion

		#region --- Ctor ---
		public AuthorPageViewModel(INavigationService navigationService, XStory.BL.Web.DSLocator.Contracts.IServiceAuthor serviceAuthor) : base(navigationService)
		{
			_serviceAuthor = serviceAuthor;

			ViewState = Helpers.ViewStateEnum.Loading;
		}
		#endregion

		private async void InitAuthor(Author author)
		{
			try
			{
				if (author == null)
				{
					ViewState = Helpers.ViewStateEnum.Error;
				}

				author = await _serviceAuthor.GetAuthorPage(StaticContext.DATASOURCE, author);

				Author = author;
				Title = Author.Name;
			}
			catch (Exception ex)
			{
				ServiceLog.Error(ex);
				ViewState = Helpers.ViewStateEnum.Error;
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
