using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.ModelBinding;
using WhatToEat.Core;
using WhatToEat.Core.Helpers;
using WhatToEat.Domain.Exceptions;
using WhatToEat.Domain.Models;

namespace WhatToEat.Domain.Services
{
    public interface ITagsService : IEntityService<RecipeTag>
    {
        Task<RecipeTag> GetOrCreateTagAsync(string name);
        Task<ICollection<RecipeTag>> ImportTagsAsync(List<string> commandTags);
    }

    public class TagsService : EntityService<RecipeTag>, ITagsService
    {
        private ILogger _logger;
        private new readonly IContext _db;

        public TagsService(IContext context) : base(context)
        {
            _db = context;
            _dbset = _db.Set<RecipeTag>();
            _logger = new DbLogger(new AppDb());
        }

        public List<RecipeTag> GetTags()
        {
            var tags = _db.RecipeTags.ToList();
            return tags;
        }

        public async Task<RecipeTag> GetOrCreateTagAsync(string name)
        {
            var tag = await _dbset
                .FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());

            if (tag != null)
                return tag;

            tag = await CreateAsync(new RecipeTag()
            {
                Name = name
            });

            return tag;
        }

        public async Task<ICollection<RecipeTag>> ImportTagsAsync(List<string> importTags)
        {
            List<RecipeTag> importedTags = new List<RecipeTag>();

            foreach (var importTag in importTags)
            {
                var tag = await GetOrCreateTagAsync(importTag);
                importedTags.Add(tag);
            }

            return importedTags;
        }
    }
}
