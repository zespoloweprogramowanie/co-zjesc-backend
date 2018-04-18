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
    public interface IRecipeCategoriesService : IEntityService<RecipeCategory>
    {
        Task<RecipeCategory> ImportCategoryAsync(string commandCategory);
    }

    public class RecipeCategoriesService : EntityService<RecipeCategory>, IRecipeCategoriesService
    {
        private ILogger _logger;
        private new readonly IContext _db;

        public RecipeCategoriesService(IContext context) : base(context)
        {
            _db = context;
            _dbset = _db.Set<RecipeCategory>();
            _logger = new DbLogger(new AppDb());
        }

        public async Task<RecipeCategory> ImportCategoryAsync(string name)
        {
            return await GetOrCreateCategoryByNameAsync(name);
        }


        public async Task<RecipeCategory> GetOrCreateCategoryByNameAsync(string name)
        {
            var category = await _dbset
                .FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());

            if (category != null)
                return category;

            category = await CreateAsync(new RecipeCategory()
            {
                Name = name
            });

            return category;
        }
    }
}
