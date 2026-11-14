using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XStory.DTO;
using XStory.Logger;
using XXRead.Core.Session;
using XXRead.Core.Sources;
using XXRead.Core.Storage;

namespace XXRead.Core.Services
{
    public class ServiceStory : IServiceStory
    {
        private IServiceSource _serviceSource;
        private IServiceSession _serviceSession;
        private IRepositoryStory _repositoryStory;

        public ServiceStory(IServiceSource serviceSource,
            IServiceSession serviceSession,
            IRepositoryStory repositoryStory)
        {
            _serviceSource = serviceSource;
            _serviceSession = serviceSession;
            _repositoryStory = repositoryStory;
        }

        private async Task<List<Story>> GetStories(int pageNumber = -1)
        {
            pageNumber = pageNumber != -1 ? pageNumber : _serviceSession.PageNumber;

            try
            {
                return await _serviceSource.Current.GetStoriesPage(pageNumber, _serviceSession.CurrentCategory?.Url);
            }
            catch (Exception ex)
            {
                // TODO : null crashes LoadMoreStories (MainPageViewModel.ExecuteLoadMoreStoriesCommand) and RefreshStories
                ServiceLog.Error(ex);
                return null;
            }
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
                ServiceLog.Error(ex);
                return null;
            }
        }

        public async Task<Story> InitStory()
        {
            try
            {
                Story story = await _serviceSource.Current.GetStory(_serviceSession.CurrentStory?.Url);
                return story;
            }
            catch (Exception ex)
            {
                ServiceLog.Error(ex);
                return null;
            }
        }

        public List<Story> DistinctStories(List<Story> stories)
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

            List<Story> storiesRefresh = await this.GetStories(pageNumber);

            if (storiesRefresh.FirstOrDefault()?.Url == firstStory.Url)
            {
                return new List<Story>();
            }

            return storiesRefresh;

        }

        public void SetCurrentStory(Story story)
        {
            _serviceSession.CurrentStory = story;
        }

        public Story GetCurrentStory()
        {
            return _serviceSession.CurrentStory;
        }

        /// <summary>
        /// Store in cache, if not already
        /// </summary>
        /// <param name="story"></param>
        public void AddAlreadyLoadedStory(Story story)
        {
            if (!_serviceSession.ListAlreadyLoadedStories.Exists(als => als.Url == story.Url))
            {
                _serviceSession.ListAlreadyLoadedStories.Add(story);
            }

        }

        public Story GetAlreadyLoadedStory(Story story)
        {
            return _serviceSession.ListAlreadyLoadedStories.FirstOrDefault(als => als.Url.Contains(story.Url));
        }

        public void SetPageNumber(int value)
        {
            _serviceSession.PageNumber = value;
        }

        public void IncrementPageNumber()
        {
            _serviceSession.PageNumber++;
        }

        public void ResetPageNumber()
        {
            _serviceSession.PageNumber = 1;
        }


        /* SQLite */

        public async Task<List<Story>> GetStoriesSQLite()
        {
            List<Story> storiesSQLite = new List<Story>();
            try
            {
                storiesSQLite = await _repositoryStory.GetStories();

                storiesSQLite = storiesSQLite.OrderByDescending(s => s.ReleaseDate).ToList();
            }
            catch (Exception ex)
            {
                ServiceLog.Error(ex);
                return null;
            }
            return storiesSQLite;
        }

        public async Task<Story> GetStorySQLite(Story story)
        {
            try
            {
                if (story == null || string.IsNullOrWhiteSpace(story.Url))
                {
                    throw new ArgumentException("Parameter is not valid.");
                }
                Story storySQLite = await _repositoryStory.GetStory(story.Url);
                return storySQLite;
            }
            catch (Exception ex)
            {
                ServiceLog.Error(ex);
                return null;
            }
        }

        public async Task<int> InsertStorySQLite(Story story)
        {
            int affectedRows;

            try
            {
                affectedRows = await _repositoryStory.InsertStory(story);
            }
            catch (Exception ex)
            {
                ServiceLog.Error(ex);
                affectedRows = -1;
            }
            return affectedRows;
        }

        public async Task<int> DeleteStorySQLite(Story story)
        {
            int affectedRows;

            try
            {
                affectedRows = await _repositoryStory.DeleteStory(story);
            }
            catch (Exception ex)
            {
                ServiceLog.Error(ex);
                affectedRows = -1;
            }
            return affectedRows;
        }
    }
}