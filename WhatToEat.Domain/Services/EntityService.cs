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
    /// <summary>
    /// Klasa bazowa dla serwisów obiektów domenowych
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class EntityService<T> : IEntityService<T> where T : class
    {
        protected IContext _db;
        protected IDbSet<T> _dbset;

        public EntityService(IContext context)
        {
            _db = context;
            _dbset = _db.Set<T>();
        }

        /// <summary>
        /// Tworzenie obiektu w bazie danych
        /// </summary>
        /// <param name="obj">Obiekt EntityFramework</param>
        /// <returns>Utworzony obiekt</returns>
        public T Create(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("entity");
            }

            _dbset.Add(obj);
            _db.SaveChanges();

            //_log.Info($"Dodano rekord {obj.GetType()}");

            return obj;
        }

        /// <summary>
        /// Asynchroniczne tworzenie obiektu w bazie danych
        /// </summary>
        /// <param name="obj">Obiekt EntityFramework<</param>
        /// <returns>Utworzony obiekt</returns>
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

        /// <summary>
        /// Usuwa obiekt z bazy danych na podstawie id
        /// </summary>
        /// <param name="id">Id obiektu</param>
        /// <returns>SaveChanges()</returns>
        public int Delete(int id)
        {
            T t = _dbset.Find(id);
            if (t == null) throw new ArgumentNullException("entity");
            _dbset.Remove(t);
            return _db.SaveChanges();
        }

        /// <summary>
        /// Usuwa obiekt z bazy danych na podstawie obiektu
        /// </summary>
        /// <param name="t">Obiekt EntityFramework</param>
        /// <returns>SaveChanges()</returns>
        public int Delete(T t)
        {
            if (t == null) throw new ArgumentNullException("entity");
            _dbset.Remove(t);
            return _db.SaveChanges();
        }

        /// <summary>
        /// Usuwa obiekt z bazy danych asynchronicznie na podstawie id
        /// </summary>
        /// <param name="id">Id obiektu</param>
        /// <returns>SaveChanges()</returns>
        public async Task<int> DeleteAsync(int id)
        {
            T t = _dbset.Find(id);
            if (t == null) throw new ArgumentNullException("entity");
            _dbset.Remove(t);
            return await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Usuwa obiekt z bazy danych asynchronicznie na podstawie obiektu
        /// </summary>
        /// <param name="t">Obiekt EntityFramework</param>
        /// <returns>SaveChanges()</returns>
        public async Task<int> DeleteAsync(T t)
        {
            if (t == null) throw new ArgumentNullException("entity");
            _dbset.Remove(t);
            return await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Pobiera wszystkie obiekty z danego DbSet
        /// </summary>
        /// <returns>Obiekty z DbSet</returns>
        public IEnumerable<T> GetAll()
        {
            return _dbset.AsEnumerable<T>();
        }

        /// <summary>
        /// Aktualizuje obiekt asynchronicznie
        /// </summary>
        /// <param name="obj">Obiekt EntityFramework</param>
        /// <returns>Zakutalizowany obiekt</returns>
        public async Task<T> UpdateAsync(T obj)
        {
            if (obj == null) throw new ArgumentNullException("entity");
            _db.Entry(obj).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return obj;
        }

        /// <summary>
        /// Szuka obiektu na podstawie id
        /// </summary>
        /// <param name="id">id obiektu</param>
        /// <returns>Obiekt EntityFramework</returns>
        public T Find(int id)
        {
            return _dbset.Find(id);
        }


        /// <summary>
        /// Szuka obiektu na podstawie id asynchronicznie
        /// </summary>
        /// <param name="id">id obiektu</param>
        /// <returns>Obiekt EntityFramework</returns>
        public Task<T> FindAsync(int id)
        {
            return _dbset.FindAsync(id);
        }

        /// <summary>
        /// Pobiera wszystkie obiekty z danego DbSet asynchronicznie
        /// </summary>
        /// <returns>Obiekty z DbSet</returns>
        public async Task<IEnumerable<T>> ListAsync()
        {
            return await _dbset.ToListAsync();
        }


        /// <summary>
        /// Aktualizuje obiekt
        /// </summary>
        /// <param name="obj">Obiekt EntityFramework</param>
        /// <returns>Zakutalizowany obiekt</returns>
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