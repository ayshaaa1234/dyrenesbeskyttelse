using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ClassLibrary.SharedKernel.Domain.Abstractions; // Opdateret for IEntity

namespace ClassLibrary.SharedKernel.Persistence.Abstractions
{
    /// <summary>
    /// Interface for generisk repository
    /// </summary>
    /// <typeparam name="T">Typen af enheden der håndteres</typeparam>
    public interface IRepository<T> where T : class, IEntity
    {
        /// <summary>
        /// Henter alle enheder
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Henter en enhed baseret på ID
        /// </summary>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Tilføjer en ny enhed
        /// </summary>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Opdaterer en eksisterende enhed
        /// </summary>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// Sletter en enhed
        /// </summary>
        Task DeleteAsync(int id);

        /// <summary>
        /// Finder enheder baseret på et predikat
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Henter sideinddelte resultater
        /// </summary>
        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? filter = null);

        /// <summary>
        /// Finder entiteter baseret på flere predikater
        /// </summary>
        Task<IEnumerable<T>> FindAllAsync(IEnumerable<Expression<Func<T, bool>>> predicates);

        /// <summary>
        /// Finder entiteter baseret på et predikat og sorterer dem
        /// </summary>
        Task<IEnumerable<T>> FindAndSortAsync<TKey>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, TKey>> sortKey,
            bool ascending = true);

        /// <summary>
        /// Finder entiteter baseret på et predikat og returnerer et bestemt antal
        /// </summary>
        Task<IEnumerable<T>> FindAndTakeAsync(
            Expression<Func<T, bool>> predicate,
            int count);

        /// <summary>
        /// Finder entiteter baseret på et predikat og springer et bestemt antal over
        /// </summary>
        Task<IEnumerable<T>> FindAndSkipAsync(
            Expression<Func<T, bool>> predicate,
            int count);

        /// <summary>
        /// Finder entiteter baseret på et predikat og grupperer dem
        /// </summary>
        Task<IDictionary<TKey, IEnumerable<T>>> FindAndGroupAsync<TKey>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, TKey>> groupKey) where TKey : notnull;
    }
} 