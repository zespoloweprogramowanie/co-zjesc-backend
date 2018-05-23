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

    /// <summary>
    /// Serwis odpowiedzialny za obsługę logiki biznesowej dla tagów
    /// </summary>
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

        /// <summary>
        /// Pobiera listę tagów
        /// </summary>
        /// <returns>Lista tagów</returns>
        public List<RecipeTag> GetTags()
        {
            var tags = _db.RecipeTags.ToList();
            return tags;
        }

        /// <summary>
        /// Tworzy lub pobiera tag na podstawie nazwy
        /// </summary>
        /// <param name="name">Nazwa tagu</param>
        /// <returns>Tag domenowy</returns>
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

        /// <summary>
        /// Importuje tag
        /// </summary>
        /// <param name="importTags">Lista tagów</param>
        /// <returns>Lista tagów domenowych</returns>
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
