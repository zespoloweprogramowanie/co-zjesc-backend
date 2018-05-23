using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatToEat.Domain.Models;

namespace WhatToEat.Domain.Services
{
    public interface ILogService : IEntityService<Log>
    {

    }

    /// <summary>
    /// Serwis odopowiedzialny za zapis elementów logu do bazy danych
    /// </summary>
    public class LogsService : EntityService<Log>, ILogService
    {
        private new IContext _db;

        public LogsService(IContext context) : base(context)
        {
            _db = context;
            _dbset = _db.Set<Log>();
        }
    }
}
