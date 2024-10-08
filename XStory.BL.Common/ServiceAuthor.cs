﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XStory.BL.Common.Contracts;
using XStory.DAL.SQLiteObjects;
using XStory.DTO;
using XStory.Logger;

namespace XStory.BL.Common
{
	public class ServiceAuthor : IServiceAuthor
	{
		private BL.Web.DSLocator.Contracts.IServiceAuthor _dsServiceAuthor;

		public ServiceAuthor(BL.Web.DSLocator.Contracts.IServiceAuthor dsServiceAuthor)
		{
			_dsServiceAuthor = dsServiceAuthor;
		}

		public void AddAlreadyLoadedAuthor(DTO.Author author)
		{
			if (StaticContext.ListAlreadyLoadedAuthors.FirstOrDefault(ala => ala.Url == author?.Url) == null)
			{
				StaticContext.ListAlreadyLoadedAuthors.Add(author);
			}
		}

		public DTO.Author GetAlreadyLoadedAuthor(DTO.Author author)
		{
			return StaticContext.ListAlreadyLoadedAuthors.FirstOrDefault(ala => ala.Url.Contains(author.Url));
		}

		public DTO.Author GetCurrentAuthor()
		{
			return StaticContext.CurrentAuthor;
		}

		public async Task<DTO.Author> InitAuthor()
		{
			try
			{
				DTO.Author author = await _dsServiceAuthor.GetAuthorPage(StaticContext.DataSource.ToString(), StaticContext.CurrentAuthor);
				return author;
			}
			catch (Exception ex)
			{
				ServiceLog.Error(ex);
				return null;
			}
		}

		public void SetCurrentAuthor(DTO.Author author)
		{
			StaticContext.CurrentAuthor = author;
		}
	}
}
