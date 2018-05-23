using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WhatToEat.Domain.Services
{
    /// <summary>
    /// Obsługuje wszystkie serwisy obiektów domenowych
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEntityService<T> : IService, IDisposable
        //where T: Base
    {
        T Create(T obj);
        Task<T> CreateAsync(T obj);
        int Delete(int id);
        int Delete(T t);
        Task<int> DeleteAsync(int id);
        Task<int> DeleteAsync(T t);
        IEnumerable<T> GetAll();
        T Update(T obj);
        Task<T> UpdateAsync(T obj);
        T Find(int id);
        Task<T> FindAsync(int id);
        Task<IEnumerable<T>> ListAsync();
    }
}