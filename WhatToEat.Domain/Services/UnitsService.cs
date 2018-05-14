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

        public List<Unit> GetUnits()
        {
            var units = _db.Units.ToList();
            return units;
        }

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
