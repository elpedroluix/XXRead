﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XStory.BL.SQLite.Contracts
{
    public interface IServiceCategory
    {
        Task<bool> HasDBCategories(string source);
        Task<List<DTO.Category>> GetCategories(string source, bool includeHidden = false);
        Task<List<string>> GetHiddenCategories(string source);
        Task<DTO.Category> GetCategory(string source, string url);
        Task<int> Save(DTO.Category category);
        Task<int> InsertCategory(DTO.Category category);
        Task<int> InsertCategories(List<DTO.Category> categories);
    }
}
