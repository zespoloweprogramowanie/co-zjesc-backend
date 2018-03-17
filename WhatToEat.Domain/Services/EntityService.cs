using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WhatToEat.Core;
using WhatToEat.Core.Extensions;
using WhatToEat.Domain.Models;

namespace WhatToEat.Domain.Services
{
    public abstract class EntityService<T> : IEntityService<T> where T : class
    {
        private ILogger _log;
        protected IContext _db;
        protected IDbSet<T> _dbset;

        public EntityService(IContext context)
        {
            _db = context;
            _dbset = _db.Set<T>();
            _log = new DbLogger(context);
        }

        public T Create(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("entity");
            }

            _dbset.Add(obj);
            _db.SaveChanges();

            _log.Info($"Dodano rekord {obj.GetType()}");

            return obj;
        }

        public async Task<T> CreateAsync(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("entity");
            }

            _dbset.Add(obj);
            await _db.SaveChangesAsync();
            return obj;
        }

        public int Delete(int id)
        {
            T t = _dbset.Find(id);
            if (t == null) throw new ArgumentNullException("entity");
            _dbset.Remove(t);
            return _db.SaveChanges();
        }

        public int Delete(T t)
        {
            if (t == null) throw new ArgumentNullException("entity");
            _dbset.Remove(t);
            return _db.SaveChanges();
        }

        public async Task<int> DeleteAsync(int id)
        {
            T t = _dbset.Find(id);
            if (t == null) throw new ArgumentNullException("entity");
            _dbset.Remove(t);
            return await _db.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(T t)
        {
            if (t == null) throw new ArgumentNullException("entity");
            _dbset.Remove(t);
            return await _db.SaveChangesAsync();
        }

        public IEnumerable<T> GetAll()
        {
            return _dbset.AsEnumerable<T>();
        }

        public async Task<T> UpdateAsync(T obj)
        {
            if (obj == null) throw new ArgumentNullException("entity");
            _db.Entry(obj).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return obj;
        }

        public T Find(int id)
        {
            return _dbset.Find(id);
        }

        public Task<T> FindAsync(int id)
        {
            return _dbset.FindAsync(id);
        }

        public async Task<IEnumerable<T>> ListAsync()
        {
            return await _dbset.ToListAsync();
        }

        public T Update(T obj)
        {
            if (obj == null) throw new ArgumentNullException("entity");
            _db.Entry(obj).State = EntityState.Modified;
            _db.SaveChanges();
            return obj;
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}