using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using XStory.BL.Common.Contracts;
using XStory.DTO;

namespace XStory.BL.Common
{
	public class ServiceStory : BL.Common.Contracts.IServiceStory
	{
		private BL.Web.DSLocator.Contracts.IServiceStory _dsServiceStory;
		

		public ServiceStory(BL.Web.DSLocator.Contracts.IServiceStory dsServiceStory)
		{
			_dsServiceStory = dsServiceStory;
		}

		private async Task<List<DTO.Story>> GetStories(int pageNumber = -1)
		{
			pageNumber = pageNumber != -1 ? pageNumber : StaticContext.PageNumber;

			var stories = await _dsServiceStory.GetStoriesPage(StaticContext.DataSource.ToString(), pageNumber, StaticContext.CurrentCategory?.Url);

			if (StaticContext.CurrentCategory == null)
			{
				// If CurrentCategory is defined, no need to filter because the category is already chosen, so valid
				stories = _dsServiceStory.FilterStories(StaticContext.DataSource.ToString(), stories, StaticContext.HiddenCategories);
			}

			return stories;
		}

		public async Task<List<Story>> InitStories()
		{
			try
			{
				List<Story> stories = await this.GetStories();

				return stories;
			}
			catch (Exception ex)
			{
				Logger.ServiceLog.Error(ex);
				return null;
			}
		}

		public List<DTO.Story> DistinctStories(List<DTO.Story> stories)
		{
			return stories.GroupBy(s => s.Url)
				.Select(s => s.First())
				.ToList();
		}

		public async Task<List<Story>> LoadMoreStories()
		{
			this.IncrementPageNumber();

			List<Story> storiesNext = await this.GetStories();

			return storiesNext;
		}

		public async Task<List<Story>> RefreshStories(Story firstStory)
		{
			int pageNumber = 1;

			List<DTO.Story> storiesRefresh = await this.GetStories(pageNumber);

			if (storiesRefresh.FirstOrDefault()?.Url == firstStory.Url)
			{
				return new List<Story>();
			}

			return storiesRefresh;

		}

		public void SetPageNumber(int value)
		{
			StaticContext.PageNumber = value;
		}

		public void IncrementPageNumber()
		{
			StaticContext.PageNumber++;
		}

		public void ResetPageNumber()
		{
			StaticContext.PageNumber = 1;
		}
	}
}