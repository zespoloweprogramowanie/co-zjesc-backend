using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using WhatToEat.Core;
using WhatToEat.Core.Helpers;
using WhatToEat.Domain.Exceptions;
using WhatToEat.Domain.Models;

namespace WhatToEat.Domain.Services
{
    public interface IRecipesService : IEntityService<Recipe>
    {
        List<string> UploadRecipeImages(IEnumerable<HttpPostedFile> files);
    }

    public class RecipesService : EntityService<Recipe>, IRecipesService
    {
        private ILogger _logger;
        private new readonly IContext _db;

        public RecipesService(IContext context) : base(context)
        {
            _db = context;
            _dbset = _db.Set<Recipe>();
            _logger = new DbLogger(new AppDb());
        }

        public List<string> UploadRecipeImages(IEnumerable<HttpPostedFile> files)
        {
            string directoryGuid = Guid.NewGuid().ToString();
            string directoryRelativePath = $@"~/Content/Images/Recipes/{directoryGuid}";
            string directoryAbsolutePath = ServerHelper.GetAbsolutePath(directoryRelativePath);

            Directory.CreateDirectory(directoryAbsolutePath);

            List<string> filePaths = new List<string>();

            foreach (var file in files)
            {
                string filePath =
                    $"{directoryAbsolutePath}/{Guid.NewGuid().ToString()}.{Path.GetExtension(file.FileName)}";

                file.SaveAs(filePath);

                filePaths.Add(filePath);
            }

            return filePaths;
        }
    }
}
