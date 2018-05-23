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
    public interface IUnitsService : IEntityService<Unit>
    {
        Task<Unit> GetOrCreateUnitByNameAsync(string unit);
        Unit GetOrCreateUnitByName(string unit);
    }

    /// <summary>
    /// Serwis odpowiedzialny za obsługę logiki biznesowej dla jednostek
    /// </summary>
    public class UnitsService : EntityService<Unit>, IUnitsService
    {
        private ILogger _logger;
        private new readonly IContext _db;

        public UnitsService(IContext context) : base(context)
        {
            _db = context;
            _dbset = _db.Set<Unit>();
            _logger = new DbLogger(new AppDb());
        }

        /// <summary>
        /// Pobiera jednostki
        /// </summary>
        /// <returns>Lista jednostek</returns>
        public List<Unit> GetUnits()
        {
            var units = _db.Units.ToList();
            return units;
        }

        /// <summary>
        /// Tworzy lub pobiera jednostkę na podstawie nazwy asynchronicznie
        /// </summary>
        /// <param name="name">Nazwa jednostki</param>
        /// <returns>Jednostka domenowa</returns>
        public async Task<Unit> GetOrCreateUnitByNameAsync(string name)
        {

            var unit = await _dbset
                .FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());

            if (unit != null)
                return unit;

            unit = await CreateAsync(new Unit()
            {
                Name = name
            });

            return unit;
        }


        /// <summary>
        /// Tworzy lub pobiera jednostkę na podstawie nazwy
        /// </summary>
        /// <param name="name">Nazwa jednostki</param>
        /// <returns>Jednostka domenowa</returns>
        public Unit GetOrCreateUnitByName(string name)
        {

            var unit = _dbset
                .FirstOrDefault(x => x.Name.ToLower() == name.ToLower());

            if (unit != null)
                return unit;

            unit = Create(new Unit()
            {
                Name = name
            });

            return unit;
        }
    }
}
